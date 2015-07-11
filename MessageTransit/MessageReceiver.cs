using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageTransit
{
    /// <summary>
    /// Message receiver to watch a channel for specific messages and notify observers
    /// </summary>
    /// <typeparam name="T">Type of message to retrieve from them channel</typeparam>
    public class MessageReceiver<T> : IObservable<T>
    {
        private readonly ITransitChannel _channel;
        private readonly IParsingStrategy<T> _parsingStrategy;
        private readonly List<IObserver<T>> _observers;
        private readonly ConcurrentQueue<T> _messagesQueue;

        private readonly object _observersLock = new object();

        readonly ManualResetEvent _stopHandle = new ManualResetEvent(false);

        /// <summary>
        /// IDisposable implementation so subscribers can unsubscribe from the message receiver
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class MessageUnsubscriber<T> : IDisposable
        {
            private readonly List<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;
            private readonly object _observersLock;


            public MessageUnsubscriber(List<IObserver<T>> observers, IObserver<T> observer, object observersLock)
            {
                _observers = observers;
                _observer = observer;
                _observersLock = observersLock;
            }


            public void Dispose()
            {
                lock (_observersLock)
                {
                    if (_observer != null && _observers.Contains(_observer))
                        _observers.Remove(_observer);    
                }
                
            }
        }

        /// <summary>
        /// Initates a message receiver to listen to a channel
        /// </summary>
        /// <param name="channel">Channel to listen on</param>
        /// <param name="parsingStrategy">Method for parsing data packets from the channel to the message</param>
        public MessageReceiver(ITransitChannel channel, IParsingStrategy<T> parsingStrategy)
        {
            _channel = channel;
            _parsingStrategy = parsingStrategy;
            _observers = new List<IObserver<T>>();
            _messagesQueue = new ConcurrentQueue<T>();

            
        }

        public void StartReceiving()
        {
            // start receiving thread
            Task.Run((Func<Task>)ReceivingTask);

            // start the publishing thread
            Task.Run((Action)PublishingTask);
        }


        /// <summary>
        /// Subscribes an observer of this message type to the receiver
        /// </summary>
        /// <param name="observer">Message processor that would receive the messages</param>
        /// <returns>Disposable token that can be used to unsubscribe the observer from this receiver</returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_observersLock)
            {
                if (!_observers.Contains(observer))
                    _observers.Add(observer);    
            }
            
            return new MessageUnsubscriber<T>(_observers, observer, _observersLock);
        }

        /// <summary>
        /// This task will listen on the channel, parse data and enqueue it for publishing
        /// </summary>
        /// <returns></returns>
        private async Task ReceivingTask()
        {
            var buffer = new byte[500 * 1024]; // 500 kilobytes max
            int packetLength = 0;
            do
            {

                packetLength = await _channel.GetNextPacket(buffer, 0);

                try
                {
                    var message = _parsingStrategy.ParseData(buffer, packetLength);
                    _messagesQueue.Enqueue(message);
                }
                catch (Exception)
                {
                    // just ignore errors for now, but we may want to make an error queue and notify observers for various errors
                }

                

            } while (packetLength > 0); // 0 length indicates end of data on the transmit channel

            // notify the publishing thread that there are no more messages
            _stopHandle.Set();
            
        }

        /// <summary>
        /// This task will publish processed messages to the subscribed message processors
        /// </summary>
        private void PublishingTask()
        {
            while (!(_stopHandle.WaitOne(300) && _messagesQueue.IsEmpty))
            {
                T message;
                while (_messagesQueue.TryDequeue(out message))
                {
                    // notify the observers that a message is available
                    foreach (var observer in _observers)
                    {
                        observer.OnNext(message);
                    }
                }
            }

            lock (_observersLock)
            {
                // notify the observers that the messages are done
                foreach (var observer in _observers.ToArray())
                {
                    observer.OnCompleted();
                }
                _observers.Clear();
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageTransit
{
    /// <summary>
    /// This is an example processor class.  It simply outputs the messages that it receives to the console
    /// </summary>
    public class SimpleMessageProcessor : IObserver<SimpleMessage>
    {
        private readonly string _processorName;

        private int messageCount = 0;

        public SimpleMessageProcessor(string processorName)
        {
            _processorName = processorName;
        }

        public virtual void OnNext(SimpleMessage value)
        {
            Console.WriteLine("{0}: {1} - {2:hh:mm:ss.fff tt}: Received message {3} of payload size {4}", _processorName, Thread.CurrentThread.ManagedThreadId, DateTime.Now, messageCount, value.MessagePayload.Length);
            messageCount ++;
        }

        public virtual void OnError(Exception error)
        {
            Console.WriteLine("{0}: {1} - {2:hh:mm:ss.fff tt}: Received error - {3}", _processorName, DateTime.Now, Thread.CurrentThread.ManagedThreadId, error);
        }

        public virtual void OnCompleted()
        {
            Console.WriteLine("{0}: {1} - {2:hh:mm:ss.fff tt} Transmission ended", _processorName, Thread.CurrentThread.ManagedThreadId, DateTime.Now);
        }
    }
}

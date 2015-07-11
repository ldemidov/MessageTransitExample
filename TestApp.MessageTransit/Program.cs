using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using MessageTransit;

namespace TestApp.MessageTransit
{
    class Program
    {
        public static void Main(string[] args)
        {

            // create a channel that will transmit 30 messages
            var transitChannel = new MockSimpleMessageTransitChannel(30);

            // setup the receiver for this channel to parse out SimpleMessages
            var messageReceiver = new MessageReceiver<SimpleMessage>(transitChannel, new SimpleMessageParsingStrategy());

            // create two processors
            var messageProcessor1 = new SimpleMessageProcessor("Processor One");
            var messageProcessor2 = new SimpleMessageProcessor("Processor Two");

            // first processor will listen on the default listening thread
            var sub1 = messageReceiver.Subscribe(messageProcessor1);
            // second processor will listen on a different thread - task pool scheduled
            var sub2 = messageReceiver
                    .ObserveOn(TaskPoolScheduler.Default)
                    .Subscribe(messageProcessor2);
            try
            {
                // start sending packets on the channel
                messageReceiver.StartReceiving();
                // prevent console app from exiting to see the results
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.Error.Write("Exception: " + ex.ToString());
            }
            finally
            {
                // clean up the receiver subscriptions
                sub1.Dispose();
                sub2.Dispose();
            }
        }
    }
}

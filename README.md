# MessageTransitExample
This project shows a simple implementation of asynchronous message processing using the Observer pattern.  Binary message data is provided by an ITransitChannel implementation.

```
public interface ITransitChannel
    {

        /// <summary>
        /// Asynchronously return the next data packet from the channel when it's available.  Return 0 when no more data is available because the channel has been closed
        /// </summary>
        /// <param name="buffer">Buffer to write the data into</param>
        /// <param name="offset">Index in the buffer to where to start writing</param>
        /// <returns>Number of bytes written to the buffer.  0 indicates nothing written because the channel has been closed</returns>
        Task<int> GetNextPacket(byte[] buffer, int offset);


    }
```

Then by implementing an IParsingStrategy to convert the binary data into a desired message object, you can create MessageReceiver object using this strategy.  Processing classes that implement the IObserver<T> class can subscribe and receive notifications from this receiver.

Example implementation.  See the TestApp.MessageTransit project for full example implementation.
```
// create a channel that will transmit 30 messages
var transitChannel = new MockSimpleMessageTransitChannel(30);

// setup the receiver for this channel to parse out SimpleMessages
var messageReceiver = new MessageReceiver<SimpleMessage>(transitChannel, new SimpleMessageParsingStrategy());

// create the processor
var messageProcessor1 = new SimpleMessageProcessor("Processor One");

// subscribe the processor
var sub1 = messageReceiver.Subscribe(messageProcessor1);

// start receiving data from the channel
messageReceiver.StartReceiving();

```
By default all message publishing is handled by one thread from the thread pool.  Since the MessageReceiver implements IObservable, I recommend using the Reactive Extensions to handle subscriptions and processing message data.  For example, to handle message processing on a new worker thread with Reactive Extensions.

```
messageReceiver
  .ObserveOn(TaskPoolScheduler.Default)
  .Subscribe(messageProcessor2);

```

## Thread safety
This project should be fully thread-safe for subscribing and receiving messages.  However the data in the messages themselves is not thread-safe by default.  Lock writes to your message objects or use immutable objects.

## Expansion ideas
  
  * Create new ITransitChannel to handle receiving data from sockets, files or other sources.
  * Notify observers for invalid data that fails parsing
  * Implement a factory of parsing strategies to handle receiving different message types on the same channel
  
  
## Recommended procedure for running the sample
1. Download repository from GitHub
2. Open solution in Visual Studio 2013
3. Restore Nuget packages if necessary
4. Compile and run the TestApp.MessageTransit project

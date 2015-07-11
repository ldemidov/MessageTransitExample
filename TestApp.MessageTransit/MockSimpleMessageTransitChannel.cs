using System;
using System.Threading;
using System.Threading.Tasks;
using MessageTransit;

namespace TestApp.MessageTransit
{
    public class MockSimpleMessageTransitChannel : ITransitChannel
    {
        private readonly int _numPackets;
        private int _packetCount = 0;
        private readonly byte[] _messageTypeInBytes;

        /// <summary>
        /// Delay between packets.  From 10 ms to this number in ms
        /// </summary>
        private const int RandomDelayMs = 200;
        

        public MockSimpleMessageTransitChannel(int numPackets)
        {
            _numPackets = numPackets;
            _messageTypeInBytes = BitConverter.GetBytes((int) 1);
        }


        public Task<int> GetNextPacket(byte[] buffer, int offset)
        {
            var rand = new Random();

            int delayMs = rand.Next(10, RandomDelayMs); // introduce an artificial delay between packets

            if (_packetCount >= _numPackets) // when transmission ended, indicate that by sending back 0 for packetLength
            {
                return Task.FromResult(0);
            }
            // otherwise create a packet with random data
            return Task.Delay(delayMs)
                .ContinueWith(task =>
                {
                    var payloadSize = rand.Next(0, 300 * 1024);
                    Array.Copy(BitConverter.GetBytes(payloadSize), 0, buffer, offset, 4);
                    Array.Copy(_messageTypeInBytes, 0, buffer, offset + 4, 4);

                    for (var i = 8; i < payloadSize + 8; ++i)
                    {
                        buffer[i] = (byte)rand.Next(255);
                    }

                    Console.WriteLine("Transmitter: {0} - {1:hh:mm:ss.fff tt}: Sending Message {2} of payload length {3}", Thread.CurrentThread.ManagedThreadId, DateTime.Now, _packetCount, payloadSize);

                    _packetCount++;
                    return payloadSize + 8;

                });

        }
    }
}
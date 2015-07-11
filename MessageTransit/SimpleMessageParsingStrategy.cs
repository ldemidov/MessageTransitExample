using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageTransit
{

    public class SimpleMessage
    {
        /// <summary>
        /// Size of the payload
        /// </summary>
        public int MessageSize { get; set; }

        /// <summary>
        /// Message type
        /// </summary>
        public int MessageType { get; set; }

        /// <summary>
        /// Raw data in the payload
        /// </summary>
        public byte[] MessagePayload { get; set; }
    }

    public class SimpleMessageParsingStrategy : IParsingStrategy<SimpleMessage>
    {

        /// <summary>
        /// Parses data from the given buffer into a message object
        /// </summary>
        /// <param name="data">Buffer to read</param>
        /// <param name="packetLength">number of bytes to read from the buffer into the message</param>
        /// <returns>Message object parsed from byte data if it's valid.  Otherwise throw an exception</returns>    
        /// <exception cref="ArgumentException">data did not match the SimpleMobile protocol</exception>
        public SimpleMessage ParseData(byte[] data, int packetLength)
        {

            int messageSize = BitConverter.ToInt32(data, 0);
            int messageType = BitConverter.ToInt32(data, 4);

            // verify that packetLength matches the SimpleMessage protocol
            // assumming message size is payload size, so packetLength should be the payload size plus header size
            if (packetLength != (messageSize + 8))
                throw new ArgumentException("Invalid packet size");

            var payload = new byte[messageSize];
            Array.Copy(data, payload, messageSize);

            return new SimpleMessage
            {
                MessageSize = messageSize,
                MessageType = messageType,
                MessagePayload = payload
            };            
        }
    }
}

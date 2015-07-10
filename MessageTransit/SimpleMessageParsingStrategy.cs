using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageTransit
{

    public class SimpleMessage
    {
        public int MessageSize { get; set; }

        public int MessageType { get; set; }

        public byte[] MessagePayload { get; set; }
    }

    public class SimpleMessageParsingStrategy : IParsingStrategy<SimpleMessage>
    {
        public SimpleMessage ParseData(byte[] data, int packetLength)
        {
            return null;
        }
    }
}

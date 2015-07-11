using System;

namespace MessageTransit
{
    public interface IParsingStrategy<out T>
    {
        /// <summary>
        /// Parses data from the given buffer into a message object
        /// </summary>
        /// <param name="data">Buffer to read</param>
        /// <param name="packetLength">number of bytes to read from the buffer into the message</param>
        /// <returns>Message object parsed from byte data if it's valid.  Otherwise throw an exception</returns>        
        T ParseData(byte[] data, int packetLength);
    

    }
}
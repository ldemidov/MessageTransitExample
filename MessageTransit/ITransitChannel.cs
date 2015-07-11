using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageTransit
{
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
    
}

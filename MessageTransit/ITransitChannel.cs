using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageTransit
{
    public interface ITransitChannel
    {

        Task<int> GetNextPacket(byte[] buffer, int offset);


    }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageTransit
{
    public class SimpleMessageProcessor<T> : IObserver<T>
    {
        public void OnNext(T value)
        {
            Console.WriteLine("{0}: Received message - {1}", DateTime.Now, value);
            
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("{0}: Received error - {1}", DateTime.Now, error);
        }

        public void OnCompleted()
        {
            Console.WriteLine("Transmission ended");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class CardsNotFoundException : Exception
    {
        public CardsNotFoundException()
        {
        }

        public CardsNotFoundException(string message) : base(message)
        {
        }

        public CardsNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

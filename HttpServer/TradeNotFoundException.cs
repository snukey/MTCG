using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class TradeNotFoundException : Exception
    {
        public TradeNotFoundException()
        {
        }

        public TradeNotFoundException(string message) : base(message)
        {
        }

        public TradeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

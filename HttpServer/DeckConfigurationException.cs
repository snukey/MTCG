using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class DeckConfigurationException : Exception
    {
        public DeckConfigurationException()
        {
        }

        public DeckConfigurationException(string message) : base(message)
        {
        }

        public DeckConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

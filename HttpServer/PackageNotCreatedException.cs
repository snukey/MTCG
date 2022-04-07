using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    [Serializable]
    public class PackageNotCreatedException : Exception
    {
        public PackageNotCreatedException()
        {

        }

        public PackageNotCreatedException(string message) : base(message)
        {
        }

        public PackageNotCreatedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

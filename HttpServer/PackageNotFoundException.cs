using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class PackageNotFoundException : Exception
    {
        public PackageNotFoundException()
        {

        }

        public PackageNotFoundException(string message) : base(message)
        {
        }

        public PackageNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

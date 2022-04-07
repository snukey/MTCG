using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core.Routing
{
    class RouteNotAuthorizedException : Exception
    {
        public RouteNotAuthorizedException()
        {
        }

        public RouteNotAuthorizedException(string message) : base(message)
        {
        }

        public RouteNotAuthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

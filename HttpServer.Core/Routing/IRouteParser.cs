using HttpServer.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpMethod = HttpServer.Core.Request.HttpMethod;

namespace HttpServer.Core.Routing
{
    public interface IRouteParser
    {
        bool IsMatch(RequestContext request, HttpMethod method, string routePattern);
        Dictionary<string, string> ParseParameters(RequestContext request, string routePattern);
    }
}

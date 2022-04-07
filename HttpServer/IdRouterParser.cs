using HttpServer.Core.Request;
using HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpServer
{
    class IdRouteParser : IRouteParser
    {
        public bool IsMatch(RequestContext request, Core.Request.HttpMethod method, string routePattern)
        {
            if (method != request.Method)
            {
                return false;
            }

            var pattern = "^" + routePattern.Replace("{id}", ".*").Replace("/", "\\/").Replace("?", "\\?") + "$";
            return Regex.IsMatch(request.ResourcePath, pattern);
        }

        public Dictionary<string, string> ParseParameters(RequestContext request, string routePattern)
        {
            var parameters = new Dictionary<string, string>();
            var pattern = "^" + routePattern.Replace("{id}", "(?<id>.*)").Replace("/", "\\/") + "$";

            var result = Regex.Match(request.ResourcePath, pattern);
            if (result.Groups["id"].Success)
            {
                parameters["id"] = result.Groups["id"].Captures[0].Value;
            }

            return parameters;
        }
    }
}

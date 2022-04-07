using HttpServer.Core.Authentication;
using HttpServer.Core.Response;
using HttpServer.Core.Routing;
using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands
{
    abstract class ProtectedRouteCommand : IProtectedRouteCommand
    {
        public IIdentity Identity { get; set; }

        public User User => (User)Identity;

        public abstract Response Execute();
    }
}

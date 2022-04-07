using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer.Core.Response;
using HttpServer.Models;

namespace HttpServer.RouteCommands.Users
{
    class GetStatsCommand : ProtectedRouteCommand
    {
        private readonly Imanager Manager;

        public GetStatsCommand(Imanager Manager)
        {
            this.Manager = Manager;
        }

        public override Response Execute()
        {
            var response = new Response();
            if (User.Username != null)
            {
                try
                {
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = Manager.GetStats(User.Username);
                }
                catch (UserNotFoundException)
                {
                    response.StatusCode = StatusCode.BadRequest;
                    response.Payload = "No user found";
                }
            }
            else
            {
                response.StatusCode = StatusCode.BadRequest;
                response.Payload = "Missing Token";
            }

            return response;
        }
    }
}

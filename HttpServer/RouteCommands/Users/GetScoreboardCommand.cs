using HttpServer.Core.Response;
using HttpServer.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Users
{
    internal class GetScoreboardCommand : ProtectedRouteCommand
    {
        private readonly Imanager Manager;

        public GetScoreboardCommand(Imanager Manager)
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
                    List<string> scores = Manager.GetScorebaord();
                    var json = JsonSerializer.Serialize(scores);
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = json;
                }
                catch (CardsNotFoundException)
                {
                    response.StatusCode = StatusCode.NotFound;
                    response.Payload = "No Deck Configured";
                    return response;
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

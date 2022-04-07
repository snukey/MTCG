using HttpServer.Core.Response;
using HttpServer.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Cards
{
    internal class ShowAcquiredCardsCommand : ProtectedRouteCommand
    {
        private readonly Imanager Manager;

        public ShowAcquiredCardsCommand(Imanager Manager)
        {
            this.Manager = Manager;
        }

        public override Response Execute()
        {
            var response = new Response();
            if(User.Username != null)
            {
                try
                {
                    List<Card> cards = new();
                    cards = Manager.ShowAcquiredCards(User.Username);
                    var json = JsonSerializer.Serialize(cards);
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = json;
                }
                catch (CardsNotFoundException)
                {

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

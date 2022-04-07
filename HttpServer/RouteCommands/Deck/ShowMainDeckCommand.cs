using HttpServer.Models;
using System.Text.Json;
using HttpServer.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Deck
{
    internal class ShowMainDeckCommand : ProtectedRouteCommand
    {
        private readonly Imanager Manager;

        public ShowMainDeckCommand(Imanager Manager)
        {
            this.Manager = Manager;
        }

        public override Response Execute()
        {
            var response = new Response();
            if (User.Username != null)
            {
                List<Card> deck = new();
                try
                {
                    deck = Manager.ShowMainDeck(User.Username);
                }
                catch (CardsNotFoundException)
                {
                    response.StatusCode = StatusCode.NotFound;
                    response.Payload = "No Deck Configured";
                    return response;
                }
        
                if (deck.Count == 4)
                {
                    var json = JsonSerializer.Serialize(deck);
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = json;
                }
                else
                {
                    response.StatusCode = StatusCode.NotFound;
                    response.Payload = "No Deck Configured";
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

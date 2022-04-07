using HttpServer.Core.Response;
using HttpServer.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Deck
{
    internal class ShowMainDeckPlainCommand : ProtectedRouteCommand
    {
        private readonly Imanager Manager;

        public ShowMainDeckPlainCommand(Imanager Manager)
        {
            this.Manager = Manager;
        }

        public override Response Execute()
        {
            var response = new Response();
            if (User.Username != null)
            {
                List<Card> Predeck = new();
                List<string> deck = new();
                try
                {
                    Predeck = Manager.ShowMainDeck(User.Username);
                    foreach(Card card in Predeck)
                    {
                        deck.Add(card.GetNameWithDamage());
                    }
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

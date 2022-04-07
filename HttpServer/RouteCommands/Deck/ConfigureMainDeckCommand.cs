using System;
using HttpServer.Models;
using HttpServer.Core.Response;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Deck
{
    internal class ConfigureMainDeckCommand : ProtectedRouteCommand
    {
        private readonly Imanager Manager;
        public List<string> cardIDs;

        public ConfigureMainDeckCommand(Imanager Manager, List<string> cardIDs)
        {
            this.Manager = Manager;
            this.cardIDs = cardIDs;
        }

        public override Response Execute()
        {
            var response = new Response();
            if (User.Username != null)
            {
                if(cardIDs.Count == 4)
                {
                    try
                    {
                        Manager.ConfigureMainDeck(User.Username, cardIDs);
                        response.StatusCode = StatusCode.Ok;
                        response.Payload = "New Deck Configured";
                    }
                    catch (DeckConfigurationException)
                    {
                        response.StatusCode = StatusCode.BadRequest;
                        response.Payload = "Couldnt Configure Deck";
                    }
                }
                else
                {
                    response.StatusCode = StatusCode.BadRequest;
                    response.Payload = "Not enough Cards sent";
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

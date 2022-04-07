using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using HttpServer.Models;
using HttpServer.Core.Response;

namespace HttpServer.RouteCommands.Trades
{
    class GetTradesCommand : ProtectedRouteCommand
    {
        private readonly Imanager manager;

        public GetTradesCommand(Imanager manager)
        {
            this.manager = manager;
        }

        public override Response Execute()
        {
            var response = new Response();
            List<Trade> trades = new();
            if (User.Username != null)
            {
                try
                {
                    trades = manager.GetTrades();
                    var json = JsonSerializer.Serialize(trades);
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = json;
                }
                catch (TradeNotFoundException)
                {
                    response.StatusCode = StatusCode.BadRequest;
                    response.Payload = "Package not found";
                }
            }
            else
            {
                
            }
            return response;
        }
    }
}

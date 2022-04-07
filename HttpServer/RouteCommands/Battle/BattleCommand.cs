using System;
using HttpServer.Core.Response;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Battle
{
    class BattleCommand : ProtectedRouteCommand
    {
        private readonly Imanager Manager;
        private readonly BattleManager battleManager;

        public BattleCommand(Imanager Manager, BattleManager battleManager)
        {
            this.Manager = Manager;
            this.battleManager = battleManager;
        }
        public override Response Execute()
        {
            Response response = new();

            var deck = Manager.ShowMainDeck(User.Username);

            if (deck.Count() != 4)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Payload = "You can't enter a battle without a configured deck!";
            }

            try
            {
                this.battleManager.RequestBattle(User, deck);
            }
            catch (DuplicateUserException)
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = "Cannot queue the same user twice!";
                return response;
            }

            System.Threading.SpinWait.SpinUntil(() => this.battleManager.isBattleFinished());
            response.StatusCode = StatusCode.Ok;
            List<string> log = this.battleManager.GetLog();
            var json = JsonSerializer.Serialize(log);
            response.StatusCode = StatusCode.Ok;
            response.Payload = json;

            return response;
        }
    }
}

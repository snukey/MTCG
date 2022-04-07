using HttpServer.Core.Response;
using HttpServer.Core.Routing;
using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Transactions
{
    class AcquirePackageCommand : ProtectedRouteCommand
    {
        private readonly Imanager manager;

        public AcquirePackageCommand(Imanager manager)
        {
            this.manager = manager;
        }

        public override Response Execute()
        {
            var response = new Response();
            List<Card> package = new();
            if(User.coins < 5)
            {
                response.StatusCode = StatusCode.PreconditionFailed;
                response.Payload = "You do not have enough money to buy a package";
            }
            else
            {
                try
                {
                    manager.AcquirePackage(User);
                    response.StatusCode = StatusCode.Ok;
                }
                catch (PackageNotFoundException)
                {
                    response.StatusCode = StatusCode.BadRequest;
                    response.Payload = "Package not found";
                }
            }
            return response;
        }
    }
}

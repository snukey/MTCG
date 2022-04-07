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
    class AddPackageCommand : ProtectedRouteCommand
    {
        //private readonly IMessageManager messageManager;
        private readonly Imanager manager;
        public List<PreCard> package;

        public AddPackageCommand(Imanager manager, List<PreCard> package)
        {
            this.manager = manager;
            this.package = package;
        }

        public override Response Execute()
        {
            var response = new Response();

            if (User.Username != "admin")
            {
                response.StatusCode = StatusCode.Forbidden;
                return response;
            }

            if(package.Count != 5)
            {
                response.StatusCode = StatusCode.PreconditionFailed;
                return response;
            }

            try
            {
                manager.AddPackage(package);
                response.StatusCode = StatusCode.Created;
            }
            catch (PackageNotCreatedException)
            {
                response.StatusCode = StatusCode.BadRequest;
                response.Payload = "Could not create Package";
            }

            return response;
        }
    }
}

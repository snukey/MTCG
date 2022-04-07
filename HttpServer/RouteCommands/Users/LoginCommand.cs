using HttpServer.Core.Response;
using HttpServer.Core.Routing;
using HttpServer.Models;
using HttpServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Users
{
    class LoginCommand : IRouteCommand
    {
        //private readonly IMessageManager messageManager;
        private readonly Imanager Manager;

        public Credentials Credentials { get; private set; }

        public LoginCommand(Imanager Manager, Credentials credentials)
        {
            Credentials = credentials;
            this.Manager = Manager;
        }

        public Response Execute()
        {
            User user;
            try
            {
                user = Manager.LoginUser(Credentials);
            }
            catch (UserNotFoundException)
            {
                user = null;
            }

            var response = new Response();
            if (user == null)
            {
                response.StatusCode = StatusCode.Unauthorized;
            }
            else
            {
                response.StatusCode = StatusCode.Ok;
                response.Payload = user.Token;
            }

            return response;
        }
    }
}

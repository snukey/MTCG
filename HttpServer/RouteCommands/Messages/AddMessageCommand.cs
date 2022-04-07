using HttpServer.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Messages
{
    class AddMessageCommand : ProtectedRouteCommand
    {
        private readonly IMessageManager messageManager;

        public string Message { get; private set; }

        public AddMessageCommand(IMessageManager messageManager, string message)
        {
            Message = message;
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var message = messageManager.AddMessage(User, Message);

            var response = new Response()
            {
                StatusCode = StatusCode.Created,
                Payload = $"{message.Id}"
            };

            return response;
        }
    }
}

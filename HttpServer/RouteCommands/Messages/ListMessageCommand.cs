using HttpServer.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Messages
{
    class ListMessagesCommand : ProtectedRouteCommand
    {
        private readonly IMessageManager messageManager;

        public ListMessagesCommand(IMessageManager messageManager)
        {
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var messages = messageManager.ListMessages(User);

            var response = new Response();

            if (messages.Any())
            {
                var payload = new StringBuilder();
                foreach (var message in messages)
                {
                    payload.Append(message.Id);
                    payload.Append(": ");
                    payload.Append(message.Content);
                    payload.Append("\n");
                }
                response.StatusCode = StatusCode.Ok;
                response.Payload = payload.ToString();
            }
            else
            {
                response.StatusCode = StatusCode.NoContent;
            }

            return response;
        }
    }
}

using HttpServer.Core.Response;
using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Messages
{
    class ShowMessageCommand : ProtectedRouteCommand
    {
        private readonly IMessageManager messageManager;

        public int MessageId { get; private set; }

        public ShowMessageCommand(IMessageManager messageManager, int messageId)
        {
            MessageId = messageId;
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            Message message;
            try
            {
                message = messageManager.ShowMessage(User, MessageId);
            }
            catch (MessageNotFoundException)
            {
                message = null;
            }

            var response = new Response();
            if (message == null)
            {
                response.StatusCode = StatusCode.NotFound;
            }
            else
            {
                response.Payload = message.Content;
                response.StatusCode = StatusCode.Ok;
            }

            return response;
        }
    }
}

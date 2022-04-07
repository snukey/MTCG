using HttpServer.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Messages
{
    class RemoveMessageCommand : ProtectedRouteCommand
    {
        private readonly IMessageManager messageManager;

        public int MessageId { get; private set; }

        public RemoveMessageCommand(IMessageManager messageManager, int messageId)
        {
            MessageId = messageId;
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            try
            {
                messageManager.RemoveMessage(User, MessageId);
                response.StatusCode = StatusCode.Ok;
            }
            catch (MessageNotFoundException)
            {
                response.StatusCode = StatusCode.NotFound;
            }

            return response;
        }
    }
}

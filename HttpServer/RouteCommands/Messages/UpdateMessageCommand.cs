using HttpServer.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.RouteCommands.Messages
{
    class UpdateMessageCommand : ProtectedRouteCommand
    {
        private readonly IMessageManager messageManager;
        public string Content { get; private set; }
        public int MessageId { get; private set; }

        public UpdateMessageCommand(IMessageManager messageManager, int messageId, string content)
        {
            MessageId = messageId;
            Content = content;
            this.messageManager = messageManager;
        }

        public override Response Execute()
        {
            var response = new Response();
            try
            {
                messageManager.UpdateMessage(User, MessageId, Content);
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

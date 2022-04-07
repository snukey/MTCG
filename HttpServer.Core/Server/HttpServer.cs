using HttpServer.Core.Client;
using HttpServer.Core.Listener;
using HttpServer.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core.Server
{
    public class HttpServer : IServer
    {
        private readonly IListener listener;
        private readonly IRouter router;
        private bool isListening;

        public HttpServer(IPAddress address, int port, IRouter router)
        {
            listener = new Listener.HttpListener(address, port);
            this.router = router;
        }

        public void Start()
        {
            listener.Start();
            isListening = true;

            while (isListening)
            {
                var client = listener.AcceptClient();
                ThreadPool.QueueUserWorkItem(HandleClient, client);
                //HandleClient(client);
            }
        }

        public void Stop()
        {
            isListening = false;
            listener.Stop();
        }

        private void HandleClient(object client)
        {
            var tcpClient = (IClient)client;
            var request = tcpClient.ReceiveRequest();

            Response.Response response;
            try
            {
                var command = router.Resolve(request);
                if (command != null)
                {
                    response = command.Execute();
                }
                else
                {
                    response = new Response.Response()
                    {
                        StatusCode = Response.StatusCode.BadRequest
                    };
                }
            }
            catch (RouteNotAuthorizedException)
            {
                response = new Response.Response()
                {
                    StatusCode = Response.StatusCode.Unauthorized
                };
            }

            tcpClient.SendResponse(response);
        }
    }
}

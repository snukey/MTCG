using HttpServer.Core.Authentication;
using HttpServer.Core.Request;
using HttpServer.DAL;
using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class MessageIdentityProvider : IIdentityProvider
    {
        private readonly IUserRepository userRepository;

        public MessageIdentityProvider(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IIdentity GetIdentyForRequest(RequestContext request)
        {
            User currentUser = null;

            if (request.Header.TryGetValue("Authorization", out string authToken))
            {
                const string prefix = "Basic ";
                if (authToken.StartsWith(prefix))
                {
                    currentUser = userRepository.GetUserByAuthToken(authToken.Substring(prefix.Length)).Result;
                }
            }
            return currentUser;
        }
    }
}

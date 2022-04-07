using HttpServer.DAL;
using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class MessageManager : IMessageManager
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;

        public MessageManager(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
        }

        public User LoginUser(Credentials credentials)
        {
            var user = userRepository.GetUserByCredentials(credentials.Username, credentials.Password);
            return user ?? throw new UserNotFoundException();
        }

        public void RegisterUser(Credentials credentials)
        {
            var user = new User()
            {
                Username = credentials.Username,
                Password = credentials.Password
            };
            if (userRepository.InsertUser(user) == false)
            {
                throw new DuplicateUserException();
            }
        }

        public Message AddMessage(User user, string content)
        {
            var message = new Message() { Content = content };
            messageRepository.InsertMessage(user.Username, message);

            return message;
        }

        public IEnumerable<Message> ListMessages(User user)
        {
            return messageRepository.GetMessages(user.Username);
        }

        public void RemoveMessage(User user, int messageId)
        {
            if (messageRepository.GetMessageById(user.Username, messageId) != null)
            {
                messageRepository.DeleteMessage(user.Username, messageId);
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public Message ShowMessage(User user, int messageId)
        {
            Message message;
            return (message = messageRepository.GetMessageById(user.Username, messageId)) != null
                ? message
                : throw new MessageNotFoundException();
        }

        public void UpdateMessage(User user, int messageId, string content)
        {
            Message message;
            if ((message = messageRepository.GetMessageById(user.Username, messageId)) != null)
            {
                message.Content = content;
                messageRepository.UpdateMessage(user.Username, message);
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }
    }
}

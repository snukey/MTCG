using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.DAL
{
    public interface IMessageRepository
    {
        IEnumerable<Message> GetMessages(string username);
        Message GetMessageById(string username, int messageId);
        void InsertMessage(string username, Message message);
        void UpdateMessage(string username, Message message);
        void DeleteMessage(string username, int messageId);
    }
}

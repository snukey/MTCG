using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.DAL
{
    public interface IUserRepository
    {
        User GetUserByCredentials(string username, string password);

        Task<User> GetUserByAuthToken(string authToken);

        bool InsertUser(User user);
        List<string> GetScoreboard();
        void UpdateScore(User winner, User loser);
        string GetStats(string username);
    }
}

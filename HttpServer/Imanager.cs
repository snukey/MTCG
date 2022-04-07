using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer.Models;
using System.Collections.Generic;

namespace HttpServer
{
    public interface Imanager
    {
        User LoginUser(Credentials credentials);
        void RegisterUser(Credentials credentials);
        void AddPackage(List<PreCard> cards);
        void AcquirePackage(User user);
        List<Card> ShowAcquiredCards(string username);
        List<Card> ShowMainDeck(string username);
        void ConfigureMainDeck(string username, List<string> cardIDs);
        string GetStats(string username);
        List<Trade> GetTrades();
        List<string> GetScorebaord();
    }
}

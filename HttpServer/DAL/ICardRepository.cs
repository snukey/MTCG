using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer.Models;

namespace HttpServer.DAL
{
    public interface ICardRepository
    {
        void AddPackage(IEnumerable<Card> package);
        void AcquireCardsFromFirstPackage(string username);
        Task<List<Card>> ShowAcquiredCards(string username);
        Task<IEnumerable<Card>> ShowMainDeck(string username);
        bool CheckCards(string username, List<String> cardIDs);
        void ConfigureMainDeck(string username, List<String> cardIDs);
    }
}

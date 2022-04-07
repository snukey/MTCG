using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer.Models;
using HttpServer.DAL;
using System.Collections.Generic;

namespace HttpServer
{
    public class Manager : Imanager
    {
        private readonly IUserRepository userRepository;
        private readonly ICardRepository cardRepository;
        private readonly ITradeRepository tradeRepository;

        public Manager(ICardRepository cardRepository, IUserRepository userRepository, ITradeRepository tradeRepository)
        {
            this.cardRepository = cardRepository;
            this.userRepository = userRepository;
            this.tradeRepository = tradeRepository;
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

        public void AddPackage(List<PreCard> cards)
        {
            List<Card> package = new();
            foreach (var card in cards)
            {
                if (card.Name.Contains("Spell"))
                {
                    Card spellCard = new Spell(card.ID, card.Name, (int) card.Damage);
                    package.Add(spellCard); 
                }
                else
                {
                    Card monsterCard = new Monster(card.ID, card.Name, (int) card.Damage);
                    package.Add(monsterCard);
                }
            }
            cardRepository.AddPackage(package);
        }
        public void AcquirePackage(User user)
        {
            if(user.coins >= 5)
            {
                cardRepository.AcquireCardsFromFirstPackage(user.Username);
            }
            //cardRepository.AcquirePackage(username, packageID);
        }

        public List<Card> ShowAcquiredCards(string username)
        {
            return cardRepository.ShowAcquiredCards(username).Result;
        }

        public List<Card> ShowMainDeck(string username)
        {
            return cardRepository.ShowMainDeck(username).Result.ToList();
        }

        public void ConfigureMainDeck(string username, List<string> cardIDs)
        {
            if(cardRepository.CheckCards(username, cardIDs))
            {
                    cardRepository.ConfigureMainDeck(username, cardIDs);
            }
        }

        public string GetStats(string username)
        {
            return userRepository.GetStats(username);
        }

        public List<Trade> GetTrades()
        {
            return tradeRepository.GetTrades();
        }

        public List<string> GetScorebaord()
        {
            return userRepository.GetScoreboard();
        }

        //public Message AddMessage(User user, string content)
        //{
        //    var message = new Message() { Content = content };
        //    messageRepository.InsertMessage(user.Username, message);

        //    return message;
        //}

        //public IEnumerable<Message> ListMessages(User user)
        //{
        //    return messageRepository.GetMessages(user.Username);
        //}

        //public void RemoveMessage(User user, int messageId)
        //{
        //    if (messageRepository.GetMessageById(user.Username, messageId) != null)
        //    {
        //        messageRepository.DeleteMessage(user.Username, messageId);
        //    }
        //    else
        //    {
        //        throw new MessageNotFoundException();
        //    }
        //}

        //public Message ShowMessage(User user, int messageId)
        //{
        //    Message message;
        //    return (message = messageRepository.GetMessageById(user.Username, messageId)) != null
        //        ? message
        //        : throw new MessageNotFoundException();
        //}

        //public void UpdateMessage(User user, int messageId, string content)
        //{
        //    Message message;
        //    if ((message = messageRepository.GetMessageById(user.Username, messageId)) != null)
        //    {
        //        message.Content = content;
        //        messageRepository.UpdateMessage(user.Username, message);
        //    }
        //    else
        //    {
        //        throw new MessageNotFoundException();
        //    }
        //}
    }
}

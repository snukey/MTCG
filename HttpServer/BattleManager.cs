using HttpServer.Models;
using HttpServer.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HttpServer
{
    public class BattleManager
    {
        private readonly IUserRepository userRepository;
        private readonly ICardRepository cardRepository;
        private readonly Game Game;
        private Queue<Tuple<User, IEnumerable<Card>>> waitlist;

        public BattleManager(ICardRepository cardRepository, IUserRepository userRepository)
        {
            this.cardRepository = cardRepository;
            this.userRepository = userRepository;
            this.Game = new Game();
            this.waitlist = new Queue<Tuple<User,IEnumerable<Card>>>();
        }

        public void RequestBattle(User user, IEnumerable<Card> deck)
        {
            if (waitlist.TryDequeue(out var queuedUser) && (queuedUser.Item1.Username == user.Username)) //same user
            {
                Console.WriteLine("Cannot queue the same user!");
                waitlist.Enqueue(queuedUser);
                throw new DuplicateUserException();
            }
            else if (queuedUser != null) //two users queued
            {
                Console.WriteLine("Two users queued! Battle can begin!");
                Game.StartGame(queuedUser.Item1, user, queuedUser.Item2.ToList(), deck.ToList());
            }
            else //first user queued & reset game
            {
                Game.ResetGame();
                waitlist.Enqueue(new Tuple<User, IEnumerable<Card>>(user, deck));
                Console.WriteLine($"User {user.Username} queued for battle!");
            }
        }
        
        public bool isBattleFinished()
        {
            if((Game.finished) && Game.Winner != null)
            {
                UpdateScore(Game.Winner, Game.Loser);
            }
            return Game.finished;
        }

        public List<string> GetLog()
        {
            return Game.GetLog();
        }
        public void UpdateScore(User winner, User loser)
        {
            this.userRepository.UpdateScore(winner, loser);
        }
    }
}

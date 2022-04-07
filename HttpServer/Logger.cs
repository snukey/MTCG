using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer.Models;

namespace HttpServer
{
    public class Logger
    {
        private List<string> log;

        public Logger()
        {
            this.log = new List<string>();
        }

        public void ResetLog()
        {
            this.log.Clear();
        }

        public void AddLog(string username1, string username2, Card card1, Card card2, double damage1, double damage2)
        {
            string log;
            log = username1 + ": " + card1.GetNameWithDamage() + " vs " + username2 + ": " + card2.GetNameWithDamage() + " => " + card1.Damage + " VS " + card2.Damage + " -> " + damage1 + " VS " + damage2 + " => ";
            if (damage1 > damage2)
            {
                log += card1.Name + "wins";    
            }
            else if(damage2 > damage1)
            {
                log += card2.Name + "wins";
            }
            else
            {
                log += "Draw (no action)\n";
            }
            this.log.Add(log);
        }

        public void AddWinner(string username)
        {
            string log;
            log = "!-!-!-!-!-!-!-!\n";
            if(username != "draw")
            {
                log += "Congratulations " + username + " you won\n";
            }
            else
            {
                log += "The game ended in a draw, congrats to both of you\n";
            }
        }

        public List<string> GetLog()
        {
            List<string> log = this.log;
            return log;
        }
    }
}

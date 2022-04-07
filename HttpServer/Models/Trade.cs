using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Models
{
    public class Trade
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string CardToTrade { get; set; }
        public CardType Type { get; set; }
        public int MinimumDamage { get; set; }
    }
}

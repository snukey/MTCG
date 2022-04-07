using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Models
{
    public class Spell : Card
    {
        public Spell(string id, string name, int damage) : base(id, name, damage)
        {
            this.Type = CardType.Spell;
            this.Name = name;
            this.ElementName = name;
        }
    }
}

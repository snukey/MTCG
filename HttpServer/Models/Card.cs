using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Models
{
    public enum Element
    {
        Normal,
        Water,
        Fire,
        Void
    };

    public enum CardType
    {
        Monster,
        Spell
    };

    public enum MonsterType
    {
        Goblin,
        Dragon,
        Wizzard,
        Ork,
        Knight,
        Kraken,
        Elf
    };
    public class Card
    {
        public string ID { get; set; }
        public Element Element { get; set; }
        public int Damage { get; set; }
        public string Name { get; set; }
        public string ElementName { get; set; }
        public CardType Type { get; set; }
        protected Card(string id, string name, int damage)
        {
            if (name.Contains("Water"))
            {
                this.Element = Element.Water;
            }
            else if (name.Contains("Fire"))
            {
                this.Element = Element.Fire;
            }
            else if (name.Contains("Void"))
            {
                this.Element = Element.Void;
            }
            else
            {
                this.Element = Element.Normal;
            }
            this.ID = id;
            this.Damage = damage;
        }

        public string GetNameWithDamage()
        {
            return this.ElementName + "(" + this.Damage + ")";
        }
    }

    public class PreCard
    {
        public string Name { get; set; }
        public float Damage { get; set; }
        public string ID { get; set; }
    }
}

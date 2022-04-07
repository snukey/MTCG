using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Models
{
    public class Monster : Card
    {
        public MonsterType MonsterType { get; set; }
        public Monster(string id, string name, int damage) : base(id, name, damage)
        {
            if (name.Contains("Goblin"))
            {
                this.MonsterType = MonsterType.Goblin;
            }
            else if (name.Contains("Dragon"))
            {
                this.MonsterType = MonsterType.Dragon;
            }
            else if (name.Contains("Wizzard"))
            {
                this.MonsterType = MonsterType.Wizzard;
            }
            else if (name.Contains("Ork"))
            {
                this.MonsterType = MonsterType.Ork;
            }
            else if (name.Contains("Knight"))
            {
                this.MonsterType = MonsterType.Knight;
            }
            else if (name.Contains("Kraken"))
            {
                this.MonsterType = MonsterType.Kraken;
            }
            else if (name.Contains("Elf"))
            {
                this.MonsterType = MonsterType.Elf;
            }
            this.Type = CardType.Monster;
            this.Name = this.MonsterType.ToString();
            this.ElementName = this.Element.ToString() + this.MonsterType.ToString();
        }
    }
}
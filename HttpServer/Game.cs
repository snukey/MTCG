using System;
using System.Collections.Generic;
using System.Linq;
using HttpServer.Models;

namespace HttpServer
{
    public class Game
    {
        public int rounds { get; set; }
        public int banishedCards { get; set; }
        public bool finished { get; set; }
        public User Winner { get; set; }
        public User Loser { get; set; }
        public Logger Logger { get; set; }

        public Game()
        {
            finished = false;
            Winner = null;
            Loser = null;
            this.banishedCards = 0;
            this.rounds = 1;
            this.Logger = new Logger();
        }

        public void ResetGame()
        {
            finished = false;
            Winner = null;
            Loser = null;
            rounds = 1;
            banishedCards = 0;
            Logger.ResetLog();
        }

        public void StartGame(User user1, User user2, List<Card> DeckUser1, List<Card> DeckUser2)
        {
            Random random = new Random();

            while(this.rounds <= 100 && DeckUser1.Count > 0 && DeckUser2.Count > 0)
            {
                int index1 = random.Next(DeckUser1.Count - 1);
                int index2 = random.Next(DeckUser2.Count - 1);
                Card cardUser1 = DeckUser1[index1];
                Card cardUser2 = DeckUser2[index2];
                (double DmgPlayer1, double DmgPlayer2) Damage = DamageCalculator(cardUser1, cardUser2);
                this.Logger.AddLog(user1.Username, user2.Username, cardUser1, cardUser2, Damage.DmgPlayer1 * cardUser1.Damage, Damage.DmgPlayer2 * cardUser1.Damage);
                double DamageCard1 = Damage.DmgPlayer1 * cardUser1.Damage;
                double DamageCard2 = Damage.DmgPlayer2 * cardUser2.Damage;
                if (DamageCard1 > DamageCard2) //Player1 card wins
                {
                    DeckUser2.RemoveAt(index2);
                    this.banishedCards++;
                    if(cardUser2.Element != Element.Void)
                    {
                        DeckUser1.Add(cardUser2);
                        this.banishedCards--;
                    }
                }
                else if(DamageCard1 < DamageCard2) //Player2 card wins
                {
                    DeckUser1.RemoveAt(index1);
                    this.banishedCards++;
                    if (cardUser1.Element != Element.Void)
                    {
                        DeckUser2.Add(cardUser1);
                        this.banishedCards--;
                    }
                }
                this.rounds++;
            }
            if(DeckUser1.Count > 0 && DeckUser2.Count == 0)
            {
                Winner = user1;
                Loser = user2;
            }
            else if(DeckUser2.Count > 0 && DeckUser1.Count == 0)
            {
                Winner = user2;
                Loser = user1;
            }
            if(Winner != null)
            {
                this.Logger.AddWinner(Winner.Username);
            }
            else
            {
                this.Logger.AddWinner("draw");
            }
            finished = true;
        }

        public (double, double) DamageCalculator(Card cardUser1, Card cardUser2)
        {
            (double Damage1, double Damage2) Damage;
            if(cardUser1.Type == CardType.Monster && cardUser2.Type == CardType.Monster)
            {
                Damage = MonsterMultiplier(cardUser1, cardUser2);
            }
            else
            {
                Damage = ElementMultiplier(cardUser1.Element, cardUser2.Element);
                (double MonsterDamage1, double MonsterDamage2) MonsterDmg = MonsterMultiplier(cardUser1, cardUser2);
                Damage = (Damage.Damage1 * MonsterDmg.MonsterDamage1, Damage.Damage2 * MonsterDmg.MonsterDamage2);

            }
            return Damage;
        }

        public (double, double) MonsterMultiplier(Card cardUser1, Card cardUser2)
        {
            double Attack1 = 1;
            double Attack2 = 1;

            if (cardUser1.Name.Contains("Goblin") && cardUser2.Name.Contains("Dragon"))
            {
                Attack1 = 0;
                Attack2 = 1;
            }
            else if (cardUser1.Name.Contains("Dragon") && cardUser2.Name.Contains("Goblin"))
            {
                Attack1 = 1;
                Attack2 = 0;
            }
            else if (cardUser1.Name.Contains("Wizzard") && cardUser2.Name.Contains("Ork"))
            {
                Attack1 = 1;
                Attack2 = 0;
            }
            else if (cardUser1.Name.Contains("Knight") && cardUser2.Name.Contains("WaterSpell"))
            {
                Attack1 = 0;
                Attack2 = 999;
            }
            else if (cardUser1.Name.Contains("WaterSpell") && cardUser2.Name.Contains("Knight"))
            {
                Attack1 = 999;
                Attack2 = 0;
            }
            else if (cardUser1.Name.Contains("Kraken") && cardUser2.Name.Contains("Spell"))
            {
                Attack1 = 1;
                Attack2 = 0;
            }
            else if (cardUser1.Name.Contains("Spell") && cardUser2.Name.Contains("Kraken"))
            {
                Attack1 = 0;
                Attack2 = 1;
            }
            else if (cardUser1.ElementName.Contains("FireElf") && cardUser2.Name.Contains("Dragon"))
            {
                Attack1 = 1;
                Attack2 = 0;
            }
            else if (cardUser1.Name.Contains("Dragon") && cardUser2.ElementName.Contains("FireElf"))
            {
                Attack1 = 0;
                Attack2 = 1;
            }

            return (Attack1, Attack2);
        }
        public (double, double)  ElementMultiplier(Element AttackElement, Element DefenceElement)
        {
            double AttackMult = 1;
            double DefenceMult = 1;

            if(AttackElement == Element.Water && DefenceElement == Element.Fire)
            {
                AttackMult = 2;
                DefenceMult = 0.5;
            }
            else if(DefenceElement == Element.Water && AttackElement == Element.Fire)
            {
                AttackMult = 0.5;
                DefenceMult = 2;

            }
            else if (AttackElement == Element.Fire && DefenceElement == Element.Normal)
            {
                AttackMult = 2;
                DefenceMult = 0.5;

            }
            else if (DefenceElement == Element.Fire && AttackElement == Element.Normal)
            {
                AttackMult = 0.5;
                DefenceMult = 2;

            }
            else if (AttackElement == Element.Normal && DefenceElement == Element.Water)
            {
                AttackMult = 2;
                DefenceMult = 0.5;

            }
            else if (DefenceElement == Element.Normal && AttackElement == Element.Water)
            {
                AttackMult = 0.5;
                DefenceMult = 2;
            }
            return (AttackMult, DefenceMult);
        }
        public List<string> GetLog()
        {
            return this.Logger.GetLog();
        }
    }
}

using NUnit.Framework;
using System.Linq;
using HttpServer;
using HttpServer.Models;
using HttpServer.DAL;
using Npgsql;

namespace HttpServer.Test
{
    [TestFixture]
    class BattleTest
    {
        private Manager manager;
        private BattleManager battleManager;
        private NpgsqlConnection connection;

        [SetUp]
        public void SetUp()
        {
            var db = new Database("Host=localhost;Port=5431;Username=postgres;Password=postgres;Database=mtcgdb");
            manager = new Manager(db.CardRepository, db.UserRepository, db.TradeRepository);
            battleManager = new BattleManager(db.CardRepository, db.UserRepository);

            connection = new NpgsqlConnection("Host=localhost;Port=5431;Username=postgres;Password=postgres;Database=mtcgdb");
            connection.Open();

            

            //create test user
        }

        [Test]
        public void MonsterVSMonsterFight()
        {
            //arrange
            var deck1 = new List<Card>()
            {
                new Monster("MvMTest1", "WaterGoblin", 20),
                new Monster("MvMTest2", "FireGoblin", 20)
            };

            var deck2 = new List<Card>()
            {
                new Monster("MvMTest3", "NormalDragen", 20),
                new Monster("MvMTest4", "WaterGoblin", 20)
            };

            var user1 = new User() { Username = "MvMUser1" };
            var user2 = new User() { Username = "MvMUser2" };

            //act
            battleManager.RequestBattle(user1, deck1);
            battleManager.RequestBattle(user2, deck2);

            //assert
            Assert.IsTrue(battleManager.isBattleFinished());
        }

        [Test]
        public void SpellVSMonsterFight()
        {
            //arrange
            var deck1 = new List<Card>()
            {
                new Monster("SvMTest1", "WaterSpell", 20),
                new Monster("SvMTest2", "FireGoblin", 20)
            };

            var deck2 = new List<Card>()
            {
                new Monster("SvMTest3", "NormalKnight", 20),
                new Monster("SvMTest4", "WaterWizzard", 20)
            };

            var user1 = new User() { Username = "SvMUser1" };
            var user2 = new User() { Username = "SvMUser2" };

            //act
            battleManager.RequestBattle(user1, deck1);
            battleManager.RequestBattle(user2, deck2);

            //assert
            Assert.IsTrue(battleManager.isBattleFinished());
        }

        [Test]
        public void SpellVSSpellFight()
        {
            //arrange
            var deck1 = new List<Card>()
            {
                new Monster("SvSTest1", "WaterSpell", 20),
                new Monster("SvSTest2", "FireSpell", 20)
            };

            var deck2 = new List<Card>()
            {
                new Monster("SvSTest3", "NormalSpell", 20),
                new Monster("SvSTest4", "NormalSpell", 20)
            };

            var user1 = new User() { Username = "SvSUser1" };
            var user2 = new User() { Username = "SvSUser2" };

            //act
            battleManager.RequestBattle(user1, deck1);
            battleManager.RequestBattle(user2, deck2);

            //assert
            Assert.IsTrue(battleManager.isBattleFinished());
        }

        [Test]
        public void VoidInteractionTest()
        {
            //arrange
            Game game = new Game();
            var deck1 = new List<Card>()
            {
                new Monster("VoidTest1", "VoidKnight", 20),
                new Monster("VoidTest2", "VoidKnight", 20)
            };

            var deck2 = new List<Card>()
            {
                new Monster("VoidTest3", "NormalSpell", 30),
                new Monster("VoidTest3", "FireDragon", 30)
            };

            var user1 = new User() { Username = "VoidUser1" };
            var user2 = new User() { Username = "VoidUser2" };

            //act
            game.StartGame(user1, user2, deck1, deck2);

            //assert
            Assert.AreEqual(2, game.banishedCards);
        }

        [Test]
        public void FightCheckDraw()
        {
            //arrange
            var deck1 = new List<Card>()
            {
                new Monster("DrawTest1", "NormalKnight", 20),
                new Monster("DrawTest2", "NormalKnight", 20)
            };

            var deck2 = new List<Card>()
            {
                new Monster("DrawTest3", "NormalKnight", 20),
                new Monster("DrawTest4", "NormalKnight", 20)
            };

            var user1 = new User() { Username = "DrawUser1" };
            var user2 = new User() { Username = "DrawUser2" };

            //act
            battleManager.RequestBattle(user1, deck1);
            battleManager.RequestBattle(user2, deck2);

            //assert
            Assert.IsTrue(battleManager.isBattleFinished());
        }

        [Test]
        [TestCase("NormalDragon", 60, "NormalOrk", 10)]
        [TestCase("NormalWizzard", 60, "NormalKnight", 10)]
        [TestCase("NormalElf", 60, "NormalKraken", 10)]
        public void TestDamageCalcWithoutMultipliers(string monster1name, int monster1damage, string monster2name, int monster2damage)
        {
            //arrange
            Game game = new Game();
            Monster card1 = new Monster("Monster1", monster1name, monster1damage);
            Monster card2 = new Monster("Monster2", monster2name, monster2damage);

            //act
            (double damageCard1, double damageCard2) damage = game.DamageCalculator(card1, card2);

            //assert
            Assert.IsTrue((damage.damageCard1 * monster1damage) > (damage.damageCard2 * monster2damage));
        }

        [Test]
        [TestCase("WaterElf", 30, "FireKnight", 50)]
        [TestCase("FireSpell", 20, "NormalGoblin", 30)]
        [TestCase("NormalElf", 40, "WaterSpell", 70)]
        public void TestDamageCalcWithElementMultiplier(string monster1name, int monster1damage, string monster2name, int monster2damage)
        {
            Game game = new Game();
            Card card1, card2;
            if (monster1name.Contains("Spell"))
            {
                card1 = new Monster("Monster1", monster1name, monster1damage);
            }
            else
            {
                card1 = new Spell("Monster1", monster1name, monster1damage);
            }
            if (monster2name.Contains("Spell"))
            {
                card2 = new Monster("Monster2", monster2name, monster2damage);
            }
            else
            {
                card2 = new Spell("Monster2", monster2name, monster2damage);
            }

            //act
            (double damageCard1, double damageCard2) damage = game.DamageCalculator(card1, card2);

            //assert
            Assert.IsTrue((damage.damageCard1 * monster1damage) > (damage.damageCard2 * monster2damage));
        }

        [Test]
        [TestCase("NormalDragon", 30, "NormalGoblin", 50)]
        [TestCase("NormalWizzard", 20, "NormalOrk", 30)]
        [TestCase("WaterSpell", 40, "NormalKnight", 70)]
        [TestCase("NormalKraken", 20, "NormalSpell", 30)]
        [TestCase("FireElf", 20, "NormalDragon", 30)]
        public void TestDamageCalcWithSpecialInteractions(string monster1name, int monster1damage, string monster2name, int monster2damage)
        {
            Game game = new Game();
            Card card1, card2;
            if (monster1name.Contains("Spell"))
            {
                card1 = new Spell("Spell1", monster1name, monster1damage);
            }
            else
            {
                card1 = new Monster("Monster1", monster1name, monster1damage);
            }
            if (monster2name.Contains("Spell"))
            {
                card2 = new Spell("Spell2", monster2name, monster2damage);
            }
            else
            {
                card2 = new Monster("Monster2", monster2name, monster2damage);
            }

            //act
            (double damageCard1, double damageCard2) damage = game.MonsterMultiplier(card1, card2);

            //assert
            Console.WriteLine(damage.damageCard1.ToString() + "\n");
            Console.WriteLine(damage.damageCard2.ToString() + "\n");
            Assert.IsTrue((damage.damageCard1 * monster1damage) > (damage.damageCard2 * monster2damage));
        }

        [Test]
        [TestCase("WaterDragon", 30, "NormalGoblin", 50)]
        [TestCase("FireWizzard", 20, "WaterOrk", 30)]
        [TestCase("WaterSpell", 40, "NormalKnight", 70)]
        [TestCase("NormalKraken", 20, "FireSpell", 30)]
        [TestCase("FireElf", 20, "WaterDragon", 30)]
        public void TestDamageCalcWithAllMultipliers(string monster1name, int monster1damage, string monster2name, int monster2damage)
        {
            Game game = new Game();
            Card card1, card2;
            if (monster1name.Contains("Spell"))
            {
                card1 = new Spell("Spell1", monster1name, monster1damage);
            }
            else
            {
                card1 = new Monster("Monster1", monster1name, monster1damage);
            }
            if (monster2name.Contains("Spell"))
            {
                card2 = new Spell("Spell2", monster2name, monster2damage);
            }
            else
            {
                card2 = new Monster("Monster2", monster2name, monster2damage);
            }

            //act
            (double damageCard1, double damageCard2) damage = game.MonsterMultiplier(card1, card2);

            //assert
            Console.WriteLine(damage.damageCard1.ToString() + "\n");
            Console.WriteLine(damage.damageCard2.ToString() + "\n");
            Assert.IsTrue((damage.damageCard1 * monster1damage) > (damage.damageCard2 * monster2damage));
        }

        [Test]
        public void TestScoreChange()
        {
            //arrange

            manager.RegisterUser(new Credentials() { Username = "HubertTest", Password = "huberttest" });
            manager.RegisterUser(new Credentials() { Username = "HelmutTest", Password = "helmuttest" });
            var user1 = new User() { Username = "HubertTest" , score = 0};
            var user2 = new User() { Username = "HelmutTest" , score = 0};

            //act
            battleManager.UpdateScore(user1, user2);

            //assert
            var command = new NpgsqlCommand("SELECT count(*) FROM users WHERE score > 100", connection);
            var result = command.ExecuteScalar();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, Convert.ToInt32(result));
        }

        [TearDown]
        public void TearDown()
        {
            var command = new NpgsqlCommand("DELETE FROM ownership;", connection);
            var command2 = new NpgsqlCommand("DELETE FROM trade;", connection);
            var command3 = new NpgsqlCommand("DELETE FROM package;", connection);
            var command4 = new NpgsqlCommand("DELETE FROM card;", connection);
            var command5 = new NpgsqlCommand("DELETE FROM users;", connection);
            command.ExecuteNonQuery();
            command2.ExecuteNonQuery();
            command3.ExecuteNonQuery();
            command4.ExecuteNonQuery();
            command5.ExecuteNonQuery();
            connection.Close();
        }
        //[Test]
        //public void CreatePackage()
        //{
        //    //arrange
        //    //act
        //    var package = PackageGenerator.GeneratePackage();

        //    //assert
        //    Assert.AreEqual(5, package.Count());
        //}

    }
}

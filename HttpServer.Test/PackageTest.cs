using NUnit.Framework;
using System.Linq;
using HttpServer;
using HttpServer.DAL;
using HttpServer.Models;
using Npgsql;

namespace HttpServer.Test
{
    [TestFixture]
    class PackageTest
    {
        private Manager manager;
        private NpgsqlConnection connection;


        [SetUp]
        public void SetUp()
        {
            var db = new Database("Host=localhost;Port=5431;Username=postgres;Password=postgres;Database=mtcgdb");
            manager = new Manager(db.CardRepository, db.UserRepository, db.TradeRepository);

            connection = new NpgsqlConnection("Host=localhost;Port=5431;Username=postgres;Password=postgres;Database=mtcgdb");
            connection.Open();
            
            //create test user
            manager.RegisterUser(new Credentials() { Username = "HerbertTest", Password = "herberttest" });
        }

        [Test]
        public void CreatePackageAndCheckInDatabase()
        {
            //arrange
            var Package = new List<PreCard>()
            {
                new PreCard()
                {
                    ID = "TestCard1",
                    Name = "FireDragon",
                    Damage = 30
                },

                new PreCard()
                {
                    ID = "TestCard2",
                    Name = "WaterSpell",
                    Damage = 10
                },

                new PreCard()
                {
                    ID = "TestCard3",
                    Name = "NormalElf",
                    Damage = 20
                },

                new PreCard()
                {
                    ID = "TestCard4",
                    Name = "WaterWizzard",
                    Damage = 30
                },

                new PreCard()
                {
                    ID = "TestCard5",
                    Name = "NormalKnight",
                    Damage = 15
                }
            };

            manager.AddPackage(Package);

            var command = new NpgsqlCommand("SELECT COUNT(*) FROM package", connection);
            var result = command.ExecuteScalar();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, Convert.ToInt32(result));
        }

        [Test]
        public void CreatePackageWithSameCardsFail()
        {
            var Package = new List<PreCard>()
            {
                new PreCard()
                {
                    ID = "TestCard1",
                    Name = "FireDragon",
                    Damage = 30
                },

                new PreCard()
                {
                    ID = "TestCard2",
                    Name = "WaterSpell",
                    Damage = 10
                },

                new PreCard()
                {
                    ID = "TestCard3",
                    Name = "NormalElf",
                    Damage = 20
                },

                new PreCard()
                {
                    ID = "TestCard4",
                    Name = "WaterWizzard",
                    Damage = 30
                },

                new PreCard()
                {
                    ID = "TestCard5",
                    Name = "NormalKnight",
                    Damage = 15
                }
            };

            manager.AddPackage(Package);

            Assert.Throws<PackageNotCreatedException>(() => manager.AddPackage(Package));
        }

        [Test]
        public void CreatePackageWithNotEnoughCardsFail()
        {
            var Package = new List<PreCard>()
            {
                new PreCard()
                {
                    ID = "TestCard1",
                    Name = "FireDragon",
                    Damage = 30
                },

                new PreCard()
                {
                    ID = "TestCard2",
                    Name = "WaterSpell",
                    Damage = 10
                },

                new PreCard()
                {
                    ID = "TestCard3",
                    Name = "NormalElf",
                    Damage = 20
                },

                new PreCard()
                {
                    ID = "TestCard4",
                    Name = "WaterWizzard",
                    Damage = 30
                }
            };

            Assert.Throws<PackageNotCreatedException>(() => manager.AddPackage(Package));
        }

        [Test]
        public void AcquireCardsFromPackageAndCheckName()
        {
            var Package = new List<PreCard>()
            {
                new PreCard()
                {
                    ID = "AcquireTestCard1",
                    Name = "FireDragon",
                    Damage = 30
                },

                new PreCard()
                {
                    ID = "AcquireTestCard2",
                    Name = "WaterSpell",
                    Damage = 10
                },

                new PreCard()
                {
                    ID = "AcquireTestCard3",
                    Name = "NormalElf",
                    Damage = 20
                },

                new PreCard()
                {
                    ID = "AcquireTestCard4",
                    Name = "WaterWizzard",
                    Damage = 30
                },

                new PreCard()
                {
                    ID = "AcquireTestCard5",
                    Name = "WaterKnight",
                    Damage = 30
                }
            };
            manager.AddPackage(Package);
            var user = new User() { Username = "HerbertTest", coins = 10 };

            manager.AcquirePackage(user);
            var command = new NpgsqlCommand("SELECT COUNT(*) FROM ownership", connection);
            var result = command.ExecuteScalar();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, Convert.ToInt32(result));
        }

        [Test]
        public void AcquireCardsFailNoCoins()
        {
            var user = new User() { Username = "HeinzTest", coins = 0 };

            manager.AcquirePackage(user);
            var command = new NpgsqlCommand("SELECT * FROM ownership", connection);
            var result = command.ExecuteScalar();

            Assert.IsNull(result);
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
    }
}

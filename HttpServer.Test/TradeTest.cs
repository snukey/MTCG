using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer.DAL;
using HttpServer;
using HttpServer.Models;
using Npgsql;
using NUnit.Framework;

namespace HttpServer.Test
{
    [TestFixture]
    class TradeTest
    {
        /*private Manager manager;
        private NpgsqlConnection connection;

        [SetUp]
        public void SetUp()
        {
            var db = new Database("Host=localhost;Port=5431;Username=postgres;Password=postgres;Database=mtcgdb");
            manager = new Manager(db.CardRepository, db.UserRepository, db.TradeRepository);

            connection = new NpgsqlConnection("Host=localhost;Port=5431;Username=postgres;Password=postgres;Database=mtcgdb");
            connection.Open();
        }

        [Test]
        public void AddTradeAndCheckDatabase()
        {
            manager.RegisterUser(new Credentials() { Username = "HeinrichTest", Password = "heinrichtest" });
            Trade trade = new Trade() { "TestTrade", "HeinrichTest", @cardToTrade, @type, @minimumDamage }
        }*/
    }
}

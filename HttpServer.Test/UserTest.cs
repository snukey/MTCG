using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer.DAL;
using HttpServer;
using HttpServer.Models;
using NUnit.Framework;
using Npgsql;

namespace HttpServer.Test
{
    [TestFixture]
    class UserTest
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
        }

        [Test]
        public void RegisterUserTest()
        {
            //arrange
            string username = "HeinrichTest";
            string password = "heinrichtest";
            var credentials = new Credentials()
            {
                Username = username,
                Password = password
            };

            //act
            manager.RegisterUser(credentials);

            var cmd = new NpgsqlCommand("SELECT password FROM users WHERE username = @user", connection);
            cmd.Parameters.AddWithValue("user", username);
            var result = cmd.ExecuteScalar();

            //assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void LoginUserSuccessfullyTest()
        {
            //arrange
            string username = "HaraldTest";
            string password = "haraldtest";
            var credentials = new Credentials()
            {
                Username = username,
                Password = password
            };

            //act
            manager.RegisterUser(credentials);
            var user = manager.LoginUser(credentials);


            //assert
            Assert.IsInstanceOf(typeof(User), user);
        }

        [Test]
        public void LoginUserFailTest()
        {
            //arrange
            string username = "HeinrichTest";
            string password = "heinrichtest";
            var credentials = new Credentials()
            {
                Username = username,
                Password = password
            };

            //act
            manager.RegisterUser(credentials);
            credentials.Password = "false";


            //assert
            Assert.Throws<UserNotFoundException>(() => manager.LoginUser(credentials));
        }

        [Test]
        public void CreateDuplicateUserTest()
        {
            //arrange
            string username = "HorstTest";
            string password = "horsttest";
            var credentials = new Credentials()
            {
                Username = "username",
                Password = "password"
            };

            //act
            manager.RegisterUser(credentials);

            //assert
            Assert.Throws<DuplicateUserException>(() => manager.RegisterUser(credentials));
        }

        [Test]
        public void GetStatsWithInvalidUser()
        {
            string username = "HansPeterTest";

            var result = manager.GetStats(username);

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

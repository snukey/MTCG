using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.DAL
{
    public class Database
    {
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlConnection _connection2;

        public IMessageRepository MessageRepository { get; private set; }
        public IUserRepository UserRepository { get; private set; }
        public ICardRepository CardRepository { get; private set; }
        public ITradeRepository TradeRepository { get; private set; }

        public Database(string connectionString)
        {
            try
            {
                _connection = new NpgsqlConnection(connectionString);
                _connection.Open();
                _connection2 = new NpgsqlConnection(connectionString);
                _connection2.Open();

                // first users, then messages
                // we need this special order since messages has a foreign key to users
                UserRepository = new DatabaseUserRepository(_connection2);
                MessageRepository = new DatabaseMessageRepository(_connection);
                CardRepository = new DatabaseCardRepository(_connection);
                TradeRepository = new DatabaseTradeRepository(_connection2);
            }
            catch (NpgsqlException e)
            {
                // provide our own custom exception
                throw new DataAccessFailedException("Could not connect to or initialize database", e);
            }
        }
    }
}

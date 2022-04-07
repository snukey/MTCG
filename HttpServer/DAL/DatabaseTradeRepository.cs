using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HttpServer.Models;
using Npgsql;

namespace HttpServer.DAL
{
     class DatabaseTradeRepository : ITradeRepository
    {
        private const string CreateTradeTableCommand = "CREATE TABLE IF NOT EXISTS trade(" +
                                                        "id VARCHAR PRIMARY KEY," +
                                                        "username VARCHAR NOT NULL REFERENCES users(username)," +
                                                        "cardToTrade VARCHAR NOT NULL REFERENCES card(id)," +
                                                        "type VARCHAR NOT NULL," +
                                                        "minimumDamage INT NOT NULL" +
                                                       ");";

        private const string InsertTradeCommand = "INSERT INTO trades VALUES(@id, @username, @cardToTrade, @type, @minimumDamage);";

        private const string GetAllTradesCommand = "SELECT * FROM trade;";

        private readonly NpgsqlConnection _connection;
        private int package_id;

        public DatabaseTradeRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            EnsureTables();
        }
        private void EnsureTables()
        {
            using var command = new NpgsqlCommand(CreateTradeTableCommand, _connection);
            command.ExecuteNonQuery();

        }
        public void InsertTrade()
        {

        }

        public List<Trade> GetTrades()
        {
            List<Trade> trades = new();
            Trade trade = new();
            using (var command = new NpgsqlCommand(GetAllTradesCommand, _connection))
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0] != DBNull.Value)
                    {
                        trade = ReadTrade(reader);
                        trades.Add(trade);
                    }
                    else
                    {
                        throw new TradeNotFoundException();
                    }
                }
            }
            return trades;
        }

        private Trade ReadTrade(IDataRecord record)
        {
        Enum.TryParse<CardType>(Convert.ToString(record["Type"]), out var cardType);
        var trade = new Trade
            {
                Id = Convert.ToString(record["Id"]),
                Username = Convert.ToString(record["Username"]),
                CardToTrade = Convert.ToString(record["CardToTrade"]),
                Type = cardType,
                MinimumDamage = Convert.ToInt32("MinimumDamage")
        };
            return trade;
        }

        //private Trade ReadTrade(IDataRecord record)
        //{
        //    Enum.TryParse<CardType>(Convert.ToString(record["card_type"]), out var cardType);
        //    Enum.TryParse<Element>(Convert.ToString(record["element"]), out var Element);
        //    Trade trade;
        //    if (cardType == CardType.Spell)
        //    {
        //        //trade = new Spell(Convert.ToString(record["id"]), Element + Convert.ToString(record["name"]), Convert.ToInt32(record["damage"]));
        //    }
        //    else
        //    {
        //        //trade = new Monster(Convert.ToString(record["id"]), Element + Convert.ToString(record["name"]), Convert.ToInt32(record["damage"]));
        //    }

        //    return trade;
        //}
    }
}

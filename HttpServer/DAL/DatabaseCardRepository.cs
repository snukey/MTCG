using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HttpServer.Models;
using Npgsql;

namespace HttpServer.DAL
{
    class DatabaseCardRepository : ICardRepository
    {
        private const string CreateCardTableCommand = "CREATE TABLE IF NOT EXISTS card(" +
                                                        "id VARCHAR PRIMARY KEY," +
                                                        "element VARCHAR NOT NULL," +
                                                        "damage INT NOT NULL," +
                                                        "name VARCHAR NOT NULL," +
                                                        "card_type VARCHAR NOT NULL," +
                                                        "monster_type VARCHAR" +
                                                        ")";

        private const string CreatePackageTableCommand = "CREATE TABLE IF NOT EXISTS package(" +
                                                            "id INT," +
                                                            "card_id VARCHAR REFERENCES card," +
                                                            "PRIMARY KEY(id, card_id)" +
                                                            ")";

        private const string CreateOwnershipTableCommand = "CREATE TABLE IF NOT EXISTS ownership(" +
                                                            "card_id VARCHAR REFERENCES card," +
                                                            "username VARCHAR REFERENCES users," +
                                                            "is_in_mainDeck BOOLEAN DEFAULT false," +
                                                            "locked BOOLEAN DEFAULT false," +
                                                            "PRIMARY KEY(card_id, username)" +
                                                            ")";

        private const string InsertPackageCommand = "INSERT INTO package(id, card_id) VALUES (@id, @card_id)";
        private const string InsertCardCommand = "INSERT INTO card(id, element, damage, name, card_type, monster_type) VALUES(@id, @element, @damage, @name, @card_type, @monster_type);";
        private const string InsertOwnershipCommand = "INSERT INTO ownership(card_id, username) VALUES (@id, @username)";

        private const string GetNextPackageIDCommand = "SELECT max(id) + 1 \"max_id\" FROM package;";
        private const string GetFirstPackageIDCommand = "SELECT min(id) \"min_id\" FROM package;";

        private const string SelectCardIDByPackageCommand = "SELECT card_id FROM package WHERE id = @id;";
        private const string SelectAllOwnershipsByUserCommand = "SELECT card_id FROM ownership WHERE username = @username;";
        private const string SelectCardCommand = "SELECT id, element, damage, name, card_type, monster_type FROM card WHERE id = @id;";
        private const string SelectMainDeckCommand = "SELECT card_id FROM ownership WHERE username = @username AND is_in_mainDeck = true;";
        private const string SelectCardsMatchingUserCommand = "SELECT card_id FROM ownership WHERE username = @username AND card_id = @card_id;";

        private const string DeleteCardFromPackageCommand = "DELETE FROM package WHERE card_id = @card_id;";

        private const string DeductMoneyCommand = "UPDATE users SET coins = (coins - @value) WHERE username = @username;";
        private const string UpdateMainDeckCommand = "UPDATE ownership SET is_in_mainDeck = @value WHERE username = @username AND card_id = @card_id";

        private readonly NpgsqlConnection _connection;
        private int package_id;
        private readonly string connectionString = "Host=localhost;Port=5431;Username=postgres;Password=postgres;Database=mtcgdb";
        public DatabaseCardRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            EnsureTables();
            using var cmd = new NpgsqlCommand(GetNextPackageIDCommand, _connection);
        }
        private void EnsureTables()
        {
            using var command = new NpgsqlCommand(CreateCardTableCommand, _connection);
            command.ExecuteNonQuery();
            using var command2 = new NpgsqlCommand(CreatePackageTableCommand, _connection);
            command2.ExecuteNonQuery();
            using var command3 = new NpgsqlCommand(CreateOwnershipTableCommand, _connection);
            command3.ExecuteNonQuery();
        }

        public void AddPackage(IEnumerable<Card> package)
        {

            //_connection.Open();
            var affectedRows = 0;
            using var transaction = _connection.BeginTransaction();

            foreach (var card in package)
            {
                try
                {
                    using var command = new NpgsqlCommand(InsertCardCommand, _connection, transaction);
                    command.Parameters.AddWithValue("id", card.ID);
                    command.Parameters.AddWithValue("element", card.Element.ToString());
                    command.Parameters.AddWithValue("damage", card.Damage);
                    command.Parameters.AddWithValue("name", card.Name);
                    command.Parameters.AddWithValue("card_type", card.Type.ToString());
                    if (card.Type == CardType.Monster)
                    {
                        command.Parameters.AddWithValue("monster_type", ((Monster)card).MonsterType.ToString());
                    }
                    else
                    {
                        command.Parameters.AddWithValue("monster_type", DBNull.Value);
                    }
                    affectedRows += command.ExecuteNonQuery();

                    using var command2 = new NpgsqlCommand(InsertPackageCommand, _connection, transaction);
                    command2.Parameters.AddWithValue("id", this.package_id);
                    command2.Parameters.AddWithValue("card_id", card.ID);
                    affectedRows += command2.ExecuteNonQuery();
                }
                catch (NpgsqlException error)
                {
                    Console.WriteLine(error.Message);
                    transaction.Rollback();
                    throw new PackageNotCreatedException();
                }
            }
            if (affectedRows != 10)
            {
                transaction.Rollback();
                throw new PackageNotCreatedException();
            }
            else
            {
                transaction.Commit();
                this.package_id++;
            }
        }

        public void AcquireCardsFromFirstPackage(string username)
        {
            List<string> cardIDs = new();
            int packageID = 0;
            try
            {
                using (var command = new NpgsqlCommand(GetFirstPackageIDCommand, _connection))
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader[0] != DBNull.Value)
                        {
                            packageID = Int32.Parse(reader[0].ToString());
                        }
                        else
                        {
                            throw new PackageNotFoundException();
                        }
                    }
                }
            }
            catch (NpgsqlException error)
            {
                Console.WriteLine(error.Message);
                throw new PackageNotFoundException();
            }
            using (var command2 = new NpgsqlCommand(SelectCardIDByPackageCommand, _connection))
            {
                command2.Parameters.AddWithValue("id", packageID);
                using var reader = command2.ExecuteReader();
                while (reader.Read())
                {
                    string cardID = reader[0].ToString();
                    cardIDs.Add(cardID);
                }
            }

            using var transaction = _connection.BeginTransaction();
            foreach (string cardID in cardIDs)
            {
                try
                {
                    using var command3 = new NpgsqlCommand(InsertOwnershipCommand, _connection, transaction);
                    command3.Parameters.AddWithValue("id", cardID);
                    command3.Parameters.AddWithValue("username", username);
                    command3.ExecuteNonQuery();

                    using var command4 = new NpgsqlCommand(DeleteCardFromPackageCommand, _connection, transaction);
                    command4.Parameters.AddWithValue("card_id", cardID);
                    command4.ExecuteNonQuery();
                }
                catch (NpgsqlException error)
                {
                    Console.WriteLine(error.Message);
                    transaction.Rollback();
                    throw new PackageNotFoundException();
                }
            }
            using var command5 = new NpgsqlCommand(DeductMoneyCommand, _connection, transaction);
            command5.Parameters.AddWithValue("value", 5);
            command5.Parameters.AddWithValue("username", username);
            command5.ExecuteNonQuery();
            transaction.Commit();
        }

        public async Task<List<Card>> ShowAcquiredCards(string username)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            List<Card> cards = new();
            List<String> cardIDs = new();
            await using(var command = new NpgsqlCommand(SelectAllOwnershipsByUserCommand, connection))
            {
                command.Parameters.AddWithValue("username", username);
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    if (reader[0] != DBNull.Value)
                    {
                        cardIDs.Add(Convert.ToString(reader[0]));
                    }
                    else
                    {
                        throw new CardsNotFoundException();
                    }
                }
            }
            foreach (string cardID in cardIDs)
            {
                await using (var command2 = new NpgsqlCommand(SelectCardCommand, connection))
                {
                    command2.Parameters.AddWithValue("id", cardID);
                    await using var reader = await command2.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        cards.Add(ReadCard(reader));
                    }
                }
            }
            return cards;
        }

        public async Task<IEnumerable<Card>> ShowMainDeck(string username)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            List<Card> cards = new();
            List<String> cardIDs = new();
            await using (var command = new NpgsqlCommand(SelectMainDeckCommand, _connection))
            {
                command.Parameters.AddWithValue("username", username);
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    if (reader[0] != DBNull.Value)
                    {
                        cardIDs.Add(Convert.ToString(reader[0]));
                    }
                    else
                    {
                        throw new CardsNotFoundException();
                    }
                }
            }
            foreach (string cardID in cardIDs)
            {
                await using (var command2 = new NpgsqlCommand(SelectCardCommand, _connection))
                {
                    command2.Parameters.AddWithValue("id", cardID);
                    await using var reader = await command2.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        cards.Add(ReadCard(reader));
                    }
                }
            }
            return cards;
        }

        public bool CheckCards(string username, List<string> cardIDs)
        {
            foreach(string cardID in cardIDs)
            {
                using (var command = new NpgsqlCommand(SelectCardsMatchingUserCommand, _connection))
                {
                    command.Parameters.AddWithValue("username", username);
                    command.Parameters.AddWithValue("card_id", cardID);
                    using var reader = command.ExecuteReader();
                    if(!reader.HasRows)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void ConfigureMainDeck(string username, List<string> cardIDs)
        {
            var affectedRows = 0;
            bool errorFlag = false;
            using var transaction = _connection.BeginTransaction();
            IEnumerable<Card> CurrentMainDeck = ShowMainDeck(username).Result;
            foreach (var card in CurrentMainDeck)
            {
                try
                {
                    using var command = new NpgsqlCommand(UpdateMainDeckCommand, _connection, transaction);
                    command.Parameters.AddWithValue("value", false);
                    command.Parameters.AddWithValue("username", username);
                    command.Parameters.AddWithValue("card_id", card.ID);
                    affectedRows += command.ExecuteNonQuery();
                }catch(NpgsqlException error)
                {
                    errorFlag = true;
                    Console.WriteLine(error.Message);
                }
                
            }
            foreach (string cardID in cardIDs)
            {
                try
                {
                    using var command2 = new NpgsqlCommand(UpdateMainDeckCommand, _connection, transaction);
                    command2.Parameters.AddWithValue("value", true);
                    command2.Parameters.AddWithValue("username", username);
                    command2.Parameters.AddWithValue("card_id", cardID);
                    affectedRows += command2.ExecuteNonQuery();
                }
                catch (NpgsqlException error)
                {
                    errorFlag = true;
                    Console.WriteLine(error.Message);
                }
            }
            if(errorFlag == true)// || affectedRows != 10)
            {
                transaction.Rollback();
                throw new DeckConfigurationException();
            }
            else
            {
                try
                {
                    transaction.Commit();
                }catch(NpgsqlException error)
                {
                    Console.WriteLine(error.Message);
                }
                
            }
        }

        private Card ReadCard(IDataRecord record)
        {
            Enum.TryParse<CardType>(Convert.ToString(record["card_type"]), out var cardType);
            Enum.TryParse<Element>(Convert.ToString(record["element"]), out var Element);
            Card card;
            if (cardType == CardType.Spell)
            {
                card = new Spell(Convert.ToString(record["id"]), Convert.ToString(record["name"]), Convert.ToInt32(record["damage"]));
            }
            else
            {
                card = new Monster(Convert.ToString(record["id"]), Element + Convert.ToString(record["name"]), Convert.ToInt32(record["damage"]));
            }

            return card;
        }

        
    }
}

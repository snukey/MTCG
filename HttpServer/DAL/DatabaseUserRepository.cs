using Npgsql;
using HttpServer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.DAL
{
    class DatabaseUserRepository : IUserRepository
    {
        private const string CreateTableCommand = "CREATE TABLE IF NOT EXISTS users (username VARCHAR PRIMARY KEY," +
                                                  "password VARCHAR, " +
                                                  "token VARCHAR," +
                                                  "coins INT DEFAULT 20," +
                                                  "score INT DEFAULT 100," +
                                                  "name VARCHAR DEFAULT ''," +
                                                  "bio VARCHAR DEFAULT ''," +
                                                  "image VARCHAR DEFAULT ''" +
                                                  ")";

        private const string InsertUserCommand = "INSERT INTO users(username, password, token) VALUES (@username, @password, @token)";

        private const string SelectUserByTokenCommand = "SELECT username, password, coins, score FROM users WHERE token=@token";
        private const string SelectUserByCredentialsCommand = "SELECT username, password, coins, score FROM users WHERE username=@username AND password=@password";
        private const string SelectStatsByUsernameCommand = "SELECT score FROM users WHERE username = @username;";

        private const string GetScoresCommand = "SELECT username, score FROM users ORDER BY score DESC;";

        private const string UpdateScoresCommand = "UPDATE users SET score = score + 10 WHERE username = @winner; UPDATE users SET score = score - 5 WHERE username = @loser;";

        private readonly NpgsqlConnection _connection;
        private readonly string connectionString = "Host=localhost;Port=5431;Username=postgres;Password=postgres;Database=mtcgdb";

        public DatabaseUserRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            EnsureTables();
        }

        public async Task<User> GetUserByAuthToken(string authToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            User user = null;
            await using (var cmd = new NpgsqlCommand(SelectUserByTokenCommand, _connection))
            {
                cmd.Parameters.AddWithValue("token", authToken);

                // take the first row, if any
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    user = ReadUser(reader);
                }
            }
            return user;
        }

        public User GetUserByCredentials(string username, string password)
        {
            User user = null;
            using (var cmd = new NpgsqlCommand(SelectUserByCredentialsCommand, _connection))
            {
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);

                // take the first row, if any
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = ReadUser(reader);
                }
            }
            return user;
        }

        public bool InsertUser(User user)
        {
            var affectedRows = 0;
            try
            {
                using var cmd = new NpgsqlCommand(InsertUserCommand, _connection);
                cmd.Parameters.AddWithValue("username", user.Username);
                cmd.Parameters.AddWithValue("password", user.Password);
                cmd.Parameters.AddWithValue("token", user.Token);
                affectedRows = cmd.ExecuteNonQuery();
            }
            catch (PostgresException)
            {
                // this might happen, if the user already exists (constraint violation)
                // we just catch it an keep affectedRows at zero
            }
            return affectedRows > 0;
        }

        private void EnsureTables()
        {
            using var cmd = new NpgsqlCommand(CreateTableCommand, _connection);
            cmd.ExecuteNonQuery();
        }

        public List<string> GetScoreboard()
        {
            List<string> scores = new();
            using (var command = new NpgsqlCommand(GetScoresCommand, _connection))

            {
                int place = 1;
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0] != DBNull.Value)
                    {
                        scores.Add(String.Format("{0}. User: {1}, Points {2}", place, Convert.ToString(reader[0]), Convert.ToString(reader[1])));
                        place++;
                    }
                    else
                    {
                        throw new CardsNotFoundException();
                    }
                }
            }
            return scores;
        }

        public void UpdateScore(User winner, User loser)
        {
            using (var command = new NpgsqlCommand(UpdateScoresCommand, _connection))
            {
                command.Parameters.AddWithValue("winner", winner.Username);
                command.Parameters.AddWithValue("loser", loser.Username);
                command.ExecuteNonQuery();
            }

        }

        public string GetStats(string username)
        {
            string stats = null;
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using (var command = new NpgsqlCommand(SelectStatsByUsernameCommand, connection))
            {
                command.Parameters.AddWithValue("username", username);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0] != DBNull.Value)
                    {
                        stats = reader[0].ToString();
                    }
                    else
                    {
                        throw new UserNotFoundException();
                    }
                }
            }
            return stats;
        }

        private User ReadUser(IDataRecord record)
        {
            var user = new User
            {
                Username = Convert.ToString(record["username"]),
                Password = Convert.ToString(record["password"]),
                coins =  Convert.ToInt32(record["coins"]),
                score = Convert.ToInt32(record["score"])
            };
            return user;
        }

       
    }
}

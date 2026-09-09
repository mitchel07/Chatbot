using System;
using MySqlConnector;

namespace CybersecurityChatbot.Wpf.Services
{
    /// <summary>
    /// Owns the MySQL connection string and makes sure the database table exists.
    /// Update the connection details below to match your local MySQL server.
    /// </summary>
    public class DatabaseService
    {
        // Change these to match your MySQL installation.
        // The database (cybersecurity_bot) is created by Database/schema.sql,
        // but EnsureSchema() will also create the tasks table if it is missing.
        public string ConnectionString { get; private set; } =
            "Server=localhost;Port=3306;Database=cybersecurity_bot;User ID=root;Password=password;";

        public DatabaseService()
        {
        }

        public DatabaseService(string connectionString)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                ConnectionString = connectionString;
            }
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        /// <summary>
        /// Opens a connection and creates the tasks table if it does not exist.
        /// Returns true when the database is reachable, false otherwise so the
        /// application can keep running (the chat features do not need the DB).
        /// </summary>
        public bool EnsureSchema(out string error)
        {
            error = string.Empty;
            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    const string sql = @"
                        CREATE TABLE IF NOT EXISTS tasks (
                            id INT AUTO_INCREMENT PRIMARY KEY,
                            title VARCHAR(255) NOT NULL,
                            description TEXT NULL,
                            reminder_date DATETIME NULL,
                            is_completed TINYINT(1) NOT NULL DEFAULT 0,
                            created_at DATETIME NOT NULL
                        );";

                    using (var cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}

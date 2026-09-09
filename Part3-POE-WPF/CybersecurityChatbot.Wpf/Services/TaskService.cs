using System;
using System.Collections.Generic;
using CybersecurityChatbot.Wpf.Models;
using MySqlConnector;

namespace CybersecurityChatbot.Wpf.Services
{
    /// <summary>
    /// Handles all cybersecurity task storage in MySQL: add, list, complete and delete.
    /// </summary>
    public class TaskService
    {
        private readonly DatabaseService _db;

        public TaskService(DatabaseService db)
        {
            _db = db;
        }

        public int Add(CyberTask task)
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();
                const string sql = @"
                    INSERT INTO tasks (title, description, reminder_date, is_completed, created_at)
                    VALUES (@title, @description, @reminder, 0, @created);
                    SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@title", task.Title ?? string.Empty);
                    cmd.Parameters.AddWithValue("@description", (object)task.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@reminder",
                        task.ReminderDate.HasValue ? (object)task.ReminderDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@created", DateTime.Now);

                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public List<CyberTask> GetAll()
        {
            var tasks = new List<CyberTask>();
            using (var connection = _db.CreateConnection())
            {
                connection.Open();
                const string sql = "SELECT id, title, description, reminder_date, is_completed, created_at FROM tasks ORDER BY id;";

                using (var cmd = new MySqlCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new CyberTask
                        {
                            Id = reader.GetInt32("id"),
                            Title = reader.GetString("title"),
                            Description = reader.IsDBNull(reader.GetOrdinal("description"))
                                ? string.Empty
                                : reader.GetString("description"),
                            ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date"))
                                ? (DateTime?)null
                                : reader.GetDateTime("reminder_date"),
                            IsCompleted = reader.GetBoolean("is_completed"),
                            CreatedAt = reader.GetDateTime("created_at")
                        });
                    }
                }
            }
            return tasks;
        }

        public void SetReminder(int taskId, DateTime reminder)
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();
                const string sql = "UPDATE tasks SET reminder_date = @reminder WHERE id = @id;";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@reminder", reminder);
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void MarkCompleted(int taskId)
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();
                const string sql = "UPDATE tasks SET is_completed = 1 WHERE id = @id;";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int taskId)
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();
                const string sql = "DELETE FROM tasks WHERE id = @id;";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

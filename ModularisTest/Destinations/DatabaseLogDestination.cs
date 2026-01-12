using ModularisTest.Enums;
using ModularisTest.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ModularisTest.Destinations
{
    public class DatabaseLogDestination : ILogDestination
    {
        private readonly string _connectionString;

        public DatabaseLogDestination(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public void LogMessage(string message, LogMessageType messageType)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var trimmedMessage = message.Trim();
            if (string.IsNullOrEmpty(trimmedMessage))
            {
                return;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("INSERT INTO Log VALUES(@message, @type, @date)", connection))
                    {
                        command.Parameters.Add("@message", SqlDbType.VarChar).Value = trimmedMessage;
                        command.Parameters.Add("@type", SqlDbType.Int).Value = (int)messageType;
                        command.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log to console if database logging fails (to avoid silent failures)
                Console.WriteLine($"Failed to log to database: {ex.Message}");
                throw;
            }
        }
    }
}

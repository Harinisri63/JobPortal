using Microsoft.Data.SqlClient;

namespace JobPortal.Database
{
    public class DatabaseConnection
    {
        private readonly string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=JobPortalDB;Trusted_Connection=True;MultipleActiveResultSets=true";

        public DatabaseConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");

            _connectionString = connectionString;
        }

        public async Task<SqlConnection> OpenAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}

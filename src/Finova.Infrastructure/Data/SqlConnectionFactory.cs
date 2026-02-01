using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Finova.Infrastructure.Data
{
    public sealed class SqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentException("Connection string is required.", nameof(connectionString));
        }

        public IDbConnection Create()
        {
            var cn = new SqlConnection(_connectionString);
            return cn;
        }
    }
}

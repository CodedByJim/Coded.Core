using System;
using System.Data;

namespace Coded.Core.Data
{
    /// <summary>
    ///     Creates connections based on the IDbConnection type and connection string
    /// </summary>
    /// <typeparam name="TDbConnection">Type of the connection</typeparam>
    public class ConnectionFactory<TDbConnection> : IConnectionFactory
        where TDbConnection : class, IDbConnection, new()
    {
        private readonly string _connectionString;

        /// <summary>
        ///     Create a new connection factory
        /// </summary>
        /// <param name="connectionString"></param>
        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <inheritdoc />
        public IDbConnection CreateConnection()
        {
            return new TDbConnection
            {
                ConnectionString = _connectionString
            };
        }
    }
}
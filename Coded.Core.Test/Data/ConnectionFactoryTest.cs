using System.Data.SQLite;
using Coded.Core.Data;
using Xunit;

namespace Coded.Core.Test.Data
{
    public class ConnectionFactoryTest
    {
        private readonly ConnectionFactory<SQLiteConnection> _connectionFactory;

        public ConnectionFactoryTest()
        {
            _connectionFactory = new ConnectionFactory<SQLiteConnection>("connection string");
        }

        [Fact(DisplayName = "Connection factory creates a connection with the given connection string.")]
        public void CreateConnection_ReturnsConnection()
        {
            //Act
            var connection = _connectionFactory.CreateConnection();

            //Assert
            Assert.NotNull(connection);
            Assert.Equal("connection string", connection.ConnectionString);
        }
    }
}
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using Coded.Core.Data;
using Moq;
using Xunit;

namespace Coded.Core.Test.Data
{
    public class DapperUnitOfWorkTest
    {
        private readonly SQLiteConnection _connection;
        private readonly DapperUnitOfWork _dapperUnitOfWork;

        public DapperUnitOfWorkTest()
        {
            _connection = new SQLiteConnection
            {
                ConnectionString = "Data Source=:memory:;Version=3;New=True;"
            };
            var connectionFactory = new Mock<IConnectionFactory>();
            connectionFactory
                .Setup(x => x.CreateConnection())
                .Returns(_connection);
            _dapperUnitOfWork = new DapperUnitOfWork(connectionFactory.Object);
        }

        [Fact(DisplayName = "Creating a dapper unit of work without connection factory throws ArgumentNullException.")]
        public void Constructor_ConnectionNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DapperUnitOfWork(null));
        }

        [Fact(DisplayName = "Closes even if the connection is null.")]
        public void Close_ClosesNullConnection()
        {
            //Act
            _dapperUnitOfWork.Close();
        }

        [Fact(DisplayName = "Closes an open connection.")]
        public void Close_ClosesConnection()
        {
            //Act
            _dapperUnitOfWork.ExecuteAsync("TestSqlValue", CancellationToken.None);
            _dapperUnitOfWork.Close();

            //Assert
            Assert.Equal(ConnectionState.Closed, _connection.State);
        }

        [Fact(DisplayName = "Executes non query.")]
        public async void ExecuteAsync_ExecutesSql()
        {
            //Act
            int result;
            using (_dapperUnitOfWork)
            {
                await _dapperUnitOfWork.ExecuteAsync(
                    "CREATE TABLE TestRecord(id INTEGER PRIMARY KEY, Value TEXT)",
                    CancellationToken.None);
                result = await _dapperUnitOfWork.ExecuteAsync("INSERT INTO TestRecord VALUES (12345, 'abc')", CancellationToken.None);
                _dapperUnitOfWork.Commit();
            }

            //Dispose twice, for test coverage
            _dapperUnitOfWork.Dispose();

            //Assert
            Assert.Equal(1, result);
        }

        [Fact(DisplayName = "Rollback works.")]
        public async void RollBack_RollsBack()
        {
            //Act
            int result;
            using (_dapperUnitOfWork)
            {
                await _dapperUnitOfWork.ExecuteAsync("CREATE TABLE TestRecord(id INTEGER PRIMARY KEY, Value TEXT)", CancellationToken.None);
                _dapperUnitOfWork.Commit();
                await _dapperUnitOfWork.ExecuteAsync("INSERT INTO TestRecord VALUES (12345, 'abc')", CancellationToken.None);
                _dapperUnitOfWork.Rollback();
                result = await _dapperUnitOfWork.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM TestRecord",
                    CancellationToken.None);
            }

            //Assert
            Assert.Equal(0, result);
        }

        [Fact(DisplayName = "Executes scalar query.")]
        public async void ExecuteScalarAsync_ExecutesSql()
        {
            //Act
            var result = await _dapperUnitOfWork.ExecuteScalarAsync<int>(
                "SELECT 12345;",
                CancellationToken.None);

            //Assert
            Assert.Equal(12345, result);
        }

        [Fact(DisplayName = "Execute single query.")]
        public async void QuerySingleOrDefaultAsync_ExecutesSql()
        {
            //Act
            var result = await _dapperUnitOfWork.QuerySingleOrDefaultAsync<TestRecord>(
                "SELECT 12345 Id, 'test' Value;",
                CancellationToken.None);

            //Assert
            Assert.Equal(12345, result.Id);
            Assert.Equal("test", result.Value);
        }

        [Fact(DisplayName = "Can execute multirow query")]
        public async void QueryAsync_ExecutesSql()
        {
            //Act
            var output = await _dapperUnitOfWork.QueryAsync<TestRecord>(
                "SELECT 12345 Id, 'test' Value UNION SELECT 67890 Id, 'item' Value;",
                CancellationToken.None);
            var result = output?.ToArray();

            //Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(12345, result[0].Id);
            Assert.Equal("test", result[0].Value);
            Assert.Equal(67890, result[1].Id);
            Assert.Equal("item", result[1].Value);
        }
    }
}
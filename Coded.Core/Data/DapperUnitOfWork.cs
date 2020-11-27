using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Coded.Core.Data
{
    /// <summary>
    ///     Represents a database transaction containing mutations that can be committed
    ///     or rolled-back atomically.
    /// </summary>
    public class DapperUnitOfWork : IUnitOfWork
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly object _connectionLock = new object();
        private readonly List<Task> _pendingQueries = new List<Task>();
        private IDbConnection _connection;
        private IDbTransaction _currentTransaction;
        private bool _disposed;

        /// <summary>
        ///     Create a new unit of work
        /// </summary>
        /// <param name="connectionFactory">The connection factory</param>
        /// <exception cref="ArgumentNullException">if the connection factory is null</exception>
        public DapperUnitOfWork(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));
            _connectionFactory = connectionFactory;
        }

        /// <inheritdoc />
        public void Commit()
        {
            try
            {
                Task.WaitAll(_pendingQueries.ToArray());
                _currentTransaction?.Commit();
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
                _pendingQueries.Clear();
            }
        }

        /// <summary>
        ///     Disposes the unit of work.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _currentTransaction?.Dispose();
            _currentTransaction = null;

            Close();
            _connection?.Dispose();
            _connection = null;

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public Task<int> ExecuteAsync(
            string sql,
            CancellationToken cancellationToken,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CommandFlags flags = CommandFlags.Buffered)
        {
            var tx = GetCurrentTransaction();
            var cmd = new CommandDefinition(sql,
                param,
                tx,
                commandTimeout,
                commandType,
                flags,
                cancellationToken);
            var task = tx.Connection.ExecuteAsync(cmd);
            _pendingQueries.Add(task);
            return task;
        }

        /// <inheritdoc />
        public Task<T> ExecuteScalarAsync<T>(
            string sql,
            CancellationToken cancellationToken,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CommandFlags flags = CommandFlags.Buffered)
        {
            var tx = GetCurrentTransaction();
            var cmd = new CommandDefinition(sql,
                param,
                tx,
                commandTimeout,
                commandType,
                flags,
                cancellationToken);
            var task = tx.Connection.ExecuteScalarAsync<T>(cmd);
            _pendingQueries.Add(task);
            return task;
        }

        /// <inheritdoc />
        public Task<T> QuerySingleOrDefaultAsync<T>(
            string sql,
            CancellationToken cancellationToken,
            object parameters = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CommandFlags flags = CommandFlags.Buffered)
        {
            var tx = GetCurrentTransaction();
            var cmd = new CommandDefinition(sql,
                parameters,
                tx,
                commandTimeout,
                commandType,
                flags,
                cancellationToken);
            var task = tx.Connection.QuerySingleOrDefaultAsync<T>(cmd);
            _pendingQueries.Add(task);
            return task;
        }

        /// <inheritdoc />
        public Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            CancellationToken cancellationToken,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CommandFlags flags = CommandFlags.Buffered)
        {
            var tx = GetCurrentTransaction();
            var cmd = new CommandDefinition(sql,
                param,
                tx,
                commandTimeout,
                commandType,
                flags,
                cancellationToken);
            var task = tx.Connection.QueryAsync<T>(cmd);
            _pendingQueries.Add(task);
            return task;
        }

        /// <inheritdoc />
        public void Rollback()
        {
            try
            {
                Task.WaitAll(_pendingQueries.ToArray());
                _currentTransaction?.Rollback();
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        /// <inheritdoc />
        public void Close()
        {
            if (_connection == null) return;

            lock (_connectionLock)
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();
        }

        private IDbTransaction GetCurrentTransaction()
        {
            lock (_connectionLock)
            {
                _connection ??= _connectionFactory.CreateConnection();
                if (_connection.State != ConnectionState.Open) _connection.Open();
                _currentTransaction ??= _connection?.BeginTransaction();
            }

            return _currentTransaction ?? throw new Exception("No transaction could be created.");
        }

        /// <summary>
        ///     Destructor, disposes the unit of work
        /// </summary>
        ~DapperUnitOfWork()
        {
            try
            {
                Dispose();
            }
            catch (Exception)
            {
                // Disposing a SQLite tx throws exceptions
                // if the connection is already disposed.
            }
        }
    }
}
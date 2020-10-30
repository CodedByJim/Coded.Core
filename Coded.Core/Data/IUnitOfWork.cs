using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Coded.Core.Data
{
    /// <summary>
    ///     Abstraction of an transaction
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        ///     Execute the sql
        /// </summary>
        /// <param name="sql">The sql</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="param">Sql parameters object</param>
        /// <param name="commandTimeout">Timeout for this command</param>
        /// <param name="commandType">Type for this command</param>
        /// <param name="flags">Command flags</param>
        /// <returns>The number of records affected</returns>
        Task<int> ExecuteAsync(
            string sql,
            CancellationToken cancellationToken,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CommandFlags flags = CommandFlags.Buffered);

        /// <summary>
        ///     Execute an sql statement and return it's scalar result
        /// </summary>
        /// <param name="sql">The sql to execute</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="param">Sql parameters object</param>
        /// <param name="commandTimeout">The timeout for this command</param>
        /// <param name="commandType">The type for this command</param>
        /// <param name="flags">Command flags</param>
        /// <typeparam name="T">The scalar result type</typeparam>
        /// <returns>A single value of type <typeparamref name="T" />.</returns>
        Task<T> ExecuteScalarAsync<T>(
            string sql,
            CancellationToken cancellationToken,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CommandFlags flags = CommandFlags.Buffered);


        /// <summary>
        ///     Execute a query and return the records.
        /// </summary>
        /// <param name="sql">The sql to execute</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="param">SQL parameters object</param>
        /// <param name="commandTimeout">The timout for this command</param>
        /// <param name="commandType">The type for this command</param>
        /// <param name="flags">Command flags</param>
        /// <typeparam name="T">Record type</typeparam>
        /// <returns>The result of the given query as an <see cref="IEnumerable{T}" /> of type <typeparamref name="T" />.</returns>
        Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            CancellationToken cancellationToken,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CommandFlags flags = CommandFlags.Buffered);

        /// <summary>
        ///     Execute a query and return a single record.
        /// </summary>
        /// <param name="sql">The sql to execute</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="param">SQL parameters object</param>
        /// <param name="commandTimeout">The timout for this command</param>
        /// <param name="commandType">The type for this command</param>
        /// <param name="flags">Command flags</param>
        /// <typeparam name="T">Record type</typeparam>
        /// <returns>The result of the given query as an <see cref="IEnumerable{T}" /> of type <typeparamref name="T" />.</returns>
        Task<T> QuerySingleOrDefaultAsync<T>(
            string sql,
            CancellationToken cancellationToken,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CommandFlags flags = CommandFlags.Buffered);

        /// <summary>
        ///     Rolls back this unit of work.
        /// </summary>
        void Rollback();

        /// <summary>
        ///     Closes the connection.
        /// </summary>
        void Close();

        /// <summary>
        ///     Commits the mutations and closes the transaction
        /// </summary>
        void Commit();
    }
}
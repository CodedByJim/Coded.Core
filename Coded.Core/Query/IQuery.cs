using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Coded.Core.Query
{
    /// <summary>
    ///     Generic query interface
    /// </summary>
    /// <typeparam name="TQueryArguments">The query arguments type</typeparam>
    /// <typeparam name="TQueryResult">The result type</typeparam>
    public interface IQuery<in TQueryArguments, TQueryResult>
        where TQueryArguments : class, IEquatable<TQueryArguments>, IQueryArguments<TQueryResult>
        where TQueryResult : class, IEquatable<TQueryResult>, new()

    {
        /// <summary>
        ///     Execute the query and return the results
        /// </summary>
        /// <param name="arguments">The query arguments</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of the query</returns>
        Task<TQueryResult?> Query([DisallowNull] TQueryArguments arguments, CancellationToken cancellationToken);
    }
}
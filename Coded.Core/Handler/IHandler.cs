using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Coded.Core.Handler
{
    /// <summary>
    ///     Handles requests.
    /// </summary>
    /// <typeparam name="TRequest">The request type to handle</typeparam>
    /// <typeparam name="TResponse">The response type to return</typeparam>
    public interface IHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IEquatable<TRequest>
        where TResponse : class, IEquatable<TResponse>, new()

    {
        /// <summary>
        ///     Handle the request and return the corresponding response
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A corresponding response</returns>
        Task<TResponse?> Handle([DisallowNull] TRequest request, CancellationToken cancellationToken);
    }
}
using System;
using Coded.Core.Query;

namespace Coded.Core.Handler
{
    /// <summary>
    ///     Request resulting in response of type <typeparamref name="TResponse" />
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequest<TResponse> : IQueryArguments<TResponse>
        where TResponse : class, IEquatable<TResponse>, new()
    {
    }
}
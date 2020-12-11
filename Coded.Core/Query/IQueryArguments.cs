using System;

namespace Coded.Core.Query
{
    /// <summary>
    ///     Arguments for query
    /// </summary>
    /// <typeparam name="TResult">The result type of this query.</typeparam>
    // ReSharper disable once UnusedTypeParameter
    public interface IQueryArguments<TResult>
        where TResult : class, IEquatable<TResult>, new()
    {
    }
}
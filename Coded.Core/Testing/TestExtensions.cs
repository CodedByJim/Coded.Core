using System;
using System.Threading;
using Coded.Core.Handler;
using Coded.Core.Query;
using Moq;

namespace Coded.Core.Testing
{
    /// <summary>
    ///     Handy extensions which can be used for unit testing purposes.
    /// </summary>
    public static class TestExtensions
    {
        /// <summary>
        ///     Setup a mock <see cref="IHandler{TRequest,TResponse}" />.
        /// </summary>
        /// <param name="handlerMock">The mock object.</param>
        /// <param name="mockMethod">The mock method implementation.</param>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        public static void SetupHandle<TRequest, TResponse>(
            this Mock<IHandler<TRequest, TResponse>> handlerMock,
            Func<TRequest, TResponse?> mockMethod)
            where TRequest : IRequest<TResponse>, IEquatable<TRequest>
            where TResponse : class, IEquatable<TResponse>, new()
        {
            handlerMock
                .Setup(handler => handler.Handle(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TRequest r, CancellationToken _) => mockMethod(r));
        }

        /// <summary>
        ///     Setup a mock <see cref="IQuery{TQueryArguments,TQueryResult}" />.
        /// </summary>
        /// <param name="queryMock">The query mock.</param>
        /// <param name="mockMethod">The mock method implementation.</param>
        /// <typeparam name="TQueryArguments">The query argument type.</typeparam>
        /// <typeparam name="TQueryResult">The query result type.</typeparam>
        public static void SetupQuery<TQueryArguments, TQueryResult>(
            this Mock<IQuery<TQueryArguments, TQueryResult>> queryMock,
            Func<TQueryArguments, TQueryResult?> mockMethod)
            where TQueryArguments : class, IQueryArguments<TQueryResult>, IEquatable<TQueryArguments>
            where TQueryResult : class, IEquatable<TQueryResult>, new()
        {
            queryMock
                .Setup(query =>
                    query.Query(
                        It.IsAny<TQueryArguments>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    (TQueryArguments queryArguments, CancellationToken _) =>
                        mockMethod(queryArguments));
        }
    }
}
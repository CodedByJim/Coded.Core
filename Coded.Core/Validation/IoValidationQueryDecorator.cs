using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Coded.Core.Query;

namespace Coded.Core.Validation
{
    /// <summary>
    ///     Decorator that validates the input and output of queries.
    /// </summary>
    /// <typeparam name="TQueryArguments">The query argument type</typeparam>
    /// <typeparam name="TResult">The query result type</typeparam>
    public class IoValidationQueryDecorator<TQueryArguments, TResult> : IQuery<TQueryArguments, TResult>
        where TQueryArguments : class, IQueryArguments<TResult>, IEquatable<TQueryArguments>
        where TResult : class, IEquatable<TResult>, new()
    {
        private readonly IQuery<TQueryArguments, TResult> _decoratedQuery;
        private readonly IValidator<TQueryArguments> _queryArgumentsValidator;
        private readonly IValidator<TResult> _queryResultValidator;

        /// <summary>
        ///     Create a new <see cref="IoValidationHandlerDecorator{TRequest,TResponse}" />
        /// </summary>
        /// <param name="decoratedQuery">The query to decorate.</param>
        /// <param name="queryArgumentsValidator">The validator for the query arguments.</param>
        /// <param name="queryResultValidator">The validator for the query result.</param>
        public IoValidationQueryDecorator(IQuery<TQueryArguments, TResult> decoratedQuery, IValidator<TQueryArguments> queryArgumentsValidator,
            IValidator<TResult> queryResultValidator)
        {
            _decoratedQuery = decoratedQuery;
            _queryArgumentsValidator = queryArgumentsValidator;
            _queryResultValidator = queryResultValidator;
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        public async Task<TResult?> Query([DisallowNull] TQueryArguments arguments, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            _queryArgumentsValidator.Validate(arguments);
            var result = await _decoratedQuery.Query(arguments, cancellationToken);

            if (result != null)
                _queryResultValidator.Validate(result);

            return result;
        }
    }
}
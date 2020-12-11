using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Coded.Core.Handler;

namespace Coded.Core.Validation
{
    /// <summary>
    ///     Handler decorator that validates input and output against it's registered validators.
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public class IoValidationHandlerDecorator<TRequest, TResponse> : IHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IEquatable<TRequest>
        where TResponse : class, IEquatable<TResponse>, new()
    {
        private readonly IHandler<TRequest, TResponse> _decoratedHandler;
        private readonly IValidator<TRequest> _requestValidator;
        private readonly IValidator<TResponse> _responseValidator;

        /// <summary>
        ///     Create a new <see cref="IoValidationHandlerDecorator{TRequest,TResponse}" />
        /// </summary>
        /// <param name="decoratedHandler">The decorated handler</param>
        /// <param name="requestValidator">The validator for the request type</param>
        /// <param name="responseValidator">The validator for the response type</param>
        public IoValidationHandlerDecorator(IHandler<TRequest, TResponse> decoratedHandler, IValidator<TRequest> requestValidator, IValidator<TResponse> responseValidator)
        {
            _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
            _requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
            _responseValidator = responseValidator ?? throw new ArgumentNullException(nameof(responseValidator));
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        public async Task<TResponse?> Handle([DisallowNull] TRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            _requestValidator.Validate(request);
            var response = await _decoratedHandler.Handle(request, cancellationToken);

            if (response != null)
                _responseValidator.Validate(response);

            return response;
        }
    }
}
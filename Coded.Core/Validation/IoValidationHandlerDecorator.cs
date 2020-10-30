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
        where TRequest : IRequest<TResponse> where TResponse : class, new()
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
            _decoratedHandler = decoratedHandler;
            _requestValidator = requestValidator;
            _responseValidator = responseValidator;
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        public async Task<TResponse> Handle([DisallowNull] TRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (request != null)
                _requestValidator.Validate(request);

            var response = await _decoratedHandler.Handle(request, cancellationToken);

            if (response != null)
                _responseValidator.Validate(response);

            return response;
        }
    }
}
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Coded.Core.Handler;

namespace Coded.Core.Events
{
    /// <summary>
    ///     Handler decorator that manages the event bus
    /// </summary>
    /// <typeparam name="TRequest">The incoming request</typeparam>
    /// <typeparam name="TResponse">The outgoing</typeparam>
    public class FlushEventBusDecorator<TRequest, TResponse> : IHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse> where TResponse : class, new()
    {
        private readonly IHandler<TRequest, TResponse> _decoratedHandler;
        private readonly IEventBus _eventBus;

        /// <summary>
        ///     Creates a new <see cref="FlushEventBusDecorator{TRequest,TResponse}" />.
        /// </summary>
        /// <param name="decoratedHandler">The decorated handler.</param>
        /// <param name="eventBus">The event bus</param>
        public FlushEventBusDecorator(IHandler<TRequest, TResponse> decoratedHandler, IEventBus eventBus)
        {
            _decoratedHandler = decoratedHandler;
            _eventBus = eventBus;
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await _decoratedHandler.Handle(request, cancellationToken);
            await _eventBus.FlushAsync(cancellationToken);
            return result;
        }
    }
}
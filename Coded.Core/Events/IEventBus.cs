using System.Threading;
using System.Threading.Tasks;

namespace Coded.Core.Events
{
    /// <summary>
    ///     Abstraction of an eventbus
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        ///     Start consuming events
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task StartConsumingAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Publish an event. Will be held in memory until it is flushed.
        /// </summary>
        /// <param name="eventObject">The event</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <returns></returns>
        Task PublishAsync<TEvent>(TEvent eventObject, CancellationToken cancellationToken)
            where TEvent : class;

        /// <summary>
        ///     Flush the published events to the eventbus.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task FlushAsync(CancellationToken cancellationToken);
    }
}
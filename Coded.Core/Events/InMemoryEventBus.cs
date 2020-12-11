using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SimpleInjector;

namespace Coded.Core.Events
{
    /// <summary>
    ///     Simple in memory event bus
    /// </summary>
    public class InMemoryEventBus : IEventBus
    {
        private readonly Container _container;
        private readonly ConcurrentQueue<object> _eventQueue = new();

        /// <summary>
        ///     Create a new <see cref="InMemoryEventBus" />.
        /// </summary>
        /// <param name="container"></param>
        public InMemoryEventBus(Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            _container = container;
        }

        /// <inheritdoc />
        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            List<Exception> exceptions = new();

            while (!_eventQueue.IsEmpty)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_eventQueue.TryDequeue(out var eventData))
                    await ConsumeEvent(eventData, exceptions, cancellationToken);
            }

            if (exceptions.Any()) throw new AggregateException(exceptions);
        }

        /// <inheritdoc />
        public Task PublishAsync<TEvent>(TEvent eventObject, CancellationToken cancellationToken)
            where TEvent : class
        {
            cancellationToken.ThrowIfCancellationRequested();
            _eventQueue.Enqueue(eventObject);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task FlushAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return StartConsumingAsync(cancellationToken);
        }

        private async Task ConsumeEvent(object eventData, ICollection<Exception> exceptions, CancellationToken cancellationToken)
        {
            var type = typeof(IConsumer<>).MakeGenericType(eventData.GetType());
            var enumerableOfType = typeof(IEnumerable<>).MakeGenericType(type);

            if (_container.GetInstance(enumerableOfType) is IEnumerable consumers)
                foreach (dynamic consumer in consumers)
                    await CallConsumer(eventData, exceptions, consumer, cancellationToken);
        }

        private static async Task CallConsumer(object eventData, ICollection<Exception> exceptions, dynamic consumer, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await consumer.ConsumeAsync((dynamic) eventData, cancellationToken);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }
    }
}
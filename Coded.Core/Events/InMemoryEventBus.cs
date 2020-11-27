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
        private readonly ConcurrentQueue<object> _eventQueue = new ConcurrentQueue<object>();

        /// <summary>
        ///     Create a new <see cref="InMemoryEventBus" />.
        /// </summary>
        /// <param name="container"></param>
        public InMemoryEventBus(Container container)
        {
            _container = container;
        }

        /// <inheritdoc />
        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var exceptions = new List<Exception>();

            while (_eventQueue.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_eventQueue.TryDequeue(out var eventData))
                    await ConsumeEvent(cancellationToken, eventData, exceptions);
            }

            if (exceptions.Any()) throw new AggregateException(exceptions);
        }

        /// <inheritdoc />
        public Task PublishAsync<TEvent>(TEvent eventObject, CancellationToken cancellationToken) where TEvent : class
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

        private async Task ConsumeEvent(CancellationToken cancellationToken, object eventData, ICollection<Exception> exceptions)
        {
            var type = typeof(IConsumer<>).MakeGenericType(eventData.GetType());
            var enumerableOfType = typeof(IEnumerable<>).MakeGenericType(type);

            if (_container.GetInstance(enumerableOfType) is IEnumerable consumers)
                foreach (dynamic consumer in consumers)
                    await CallConsumer(cancellationToken, eventData, exceptions, consumer);
        }

        private static async Task CallConsumer(CancellationToken cancellationToken, object eventData, ICollection<Exception> exceptions, dynamic consumer)
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
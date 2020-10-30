using System.Threading;
using System.Threading.Tasks;

namespace Coded.Core.Events
{
    /// <summary>
    ///     Consumers are called when events of their consuming type happen.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IConsumer<in TEvent>
    {
        /// <summary>
        ///     Process the event
        /// </summary>
        /// <param name="eventObject">The event object</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
        Task ConsumeAsync(TEvent eventObject, CancellationToken cancellationToken);
    }
}
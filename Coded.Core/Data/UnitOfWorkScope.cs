using System.Threading;
using System.Threading.Tasks;
using Coded.Core.Handler;

namespace Coded.Core.Data
{
    /// <summary>
    ///     Handler decorator that manages a unit of work scope per request
    /// </summary>
    /// <typeparam name="TRequest">The incoming request type</typeparam>
    /// <typeparam name="TResponse">The outgoing response type</typeparam>
    public class UnitOfWorkScope<TRequest, TResponse> : IHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse> where TResponse : class, new()
    {
        private readonly IHandler<TRequest, TResponse> _decoratedHandler;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Create a new unit of work scope
        /// </summary>
        /// <param name="decoratedHandler">The inner handler</param>
        /// <param name="unitOfWork">The unit of work</param>
        public UnitOfWorkScope(IHandler<TRequest, TResponse> decoratedHandler, IUnitOfWork unitOfWork)
        {
            _decoratedHandler = decoratedHandler;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            using (_unitOfWork)
            {
                var result = await _decoratedHandler.Handle(request, cancellationToken);
                _unitOfWork.Commit();
                _unitOfWork.Close();
                return result;
            }
        }
    }
}
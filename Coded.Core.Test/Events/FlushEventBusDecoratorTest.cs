using System.Threading;
using System.Threading.Tasks;
using Coded.Core.Events;
using Coded.Core.Handler;
using Coded.Core.Testing;
using Moq;
using Xunit;

namespace Coded.Core.Test.Events
{
    public class FlushEventBusDecoratorTest
    {
        private readonly FlushEventBusDecorator<TestRequest, TestResponse> _decorator;
        private readonly Mock<IEventBus> _eventBust;
        private readonly Mock<IHandler<TestRequest, TestResponse>> _innerHandler;

        public FlushEventBusDecoratorTest()
        {
            _eventBust = new();
            _innerHandler = new();
            _decorator = new(
                _innerHandler.Object,
                _eventBust.Object);
        }

        [Fact(DisplayName = "Decorator calls inner handler and flushes event bus.")]
        public async Task Decorator_FlushesEventBus()
        {
            //Arrange
            TestResponse response = new();
            _innerHandler.SetupHandle(_ => response);

            TestRequest request = new();

            //Act
            await _decorator.Handle(request, CancellationToken.None);

            //Assert
            _innerHandler.Verify(x => x.Handle(request, CancellationToken.None));
            _eventBust.Verify(x => x.FlushAsync(CancellationToken.None));
        }
    }
}
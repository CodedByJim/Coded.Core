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
            _eventBust = new Mock<IEventBus>();
            _innerHandler = new Mock<IHandler<TestRequest, TestResponse>>();
            _decorator = new FlushEventBusDecorator<TestRequest, TestResponse>(
                _innerHandler.Object,
                _eventBust.Object);
        }

        [Fact(DisplayName = "Decorator calls inner handler and flushes event bus.")]
        public async Task Decorator_FlushesEventBus()
        {
            //Arrange
            var response = new TestResponse();
            _innerHandler.SetupHandle(_ => response);

            var request = new TestRequest();

            //Act
            await _decorator.Handle(request, CancellationToken.None);

            //Assert
            _innerHandler.Verify(x => x.Handle(request, CancellationToken.None));
            _eventBust.Verify(x => x.FlushAsync(CancellationToken.None));
        }
    }
}
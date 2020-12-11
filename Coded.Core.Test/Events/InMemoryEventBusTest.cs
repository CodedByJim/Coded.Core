using System;
using System.Threading;
using System.Threading.Tasks;
using Coded.Core.Events;
using Moq;
using SimpleInjector;
using Xunit;

namespace Coded.Core.Test.Events
{
    public class InMemoryEventBusTest
    {
        private readonly InMemoryEventBus _eventBus;
        private readonly Mock<IConsumer<TestRecord>> _mockConsumer1;
        private readonly Mock<IConsumer<TestRecord>> _mockConsumer2;

        public InMemoryEventBusTest()
        {
            _mockConsumer1 = new();
            _mockConsumer2 = new();

            Container container = new();
            container.Collection.Register(typeof(IConsumer<TestRecord>),
                new[]
                {
                    _mockConsumer1.Object,
                    _mockConsumer2.Object
                });
            container.Verify();

            _eventBus = new(container);
        }

        [Fact(DisplayName = "Event gets published.")]
        public async Task Event_Publish()
        {
            await _eventBus.PublishAsync(
                new TestRecord(),
                CancellationToken.None);
        }

        [Fact(DisplayName = "Consumer exceptions get aggregated.")]
        public async Task ConsumerExceptions_GetAggregated()
        {
            //Arrange
            _mockConsumer1.Setup(x => x.ConsumeAsync(
                    It.IsAny<TestRecord>(),
                    CancellationToken.None))
                .Throws(new("Exception1"));
            _mockConsumer2.Setup(x => x.ConsumeAsync(
                    It.IsAny<TestRecord>(),
                    CancellationToken.None))
                .Throws(new("Exception2"));

            await _eventBus.PublishAsync(
                new TestRecord(),
                CancellationToken.None);

            //Act
            var exception = await Assert.ThrowsAsync<AggregateException>(async () =>
                await _eventBus.StartConsumingAsync(CancellationToken.None));

            //Assert
            Assert.Equal("Exception1", exception.InnerExceptions[0].Message);
            Assert.Equal("Exception2", exception.InnerExceptions[1].Message);
        }

        [Fact(DisplayName = "Events get consumed.")]
        public async Task Events_GetConsumed()
        {
            //Arrange
            TestRecord testEvent1 = new()
            {
                Id = 123,
                Value = "Value1"
            };

            TestRecord testEvent2 = new()
            {
                Id = 456,
                Value = "Value2"
            };

            await _eventBus.PublishAsync(
                testEvent1,
                CancellationToken.None);

            await _eventBus.PublishAsync(
                testEvent2,
                CancellationToken.None);

            //Act
            await _eventBus.FlushAsync(CancellationToken.None);

            //Assert
            _mockConsumer1.Verify(x =>
                x.ConsumeAsync(testEvent1, CancellationToken.None), Times.Once);
            _mockConsumer1.Verify(x =>
                x.ConsumeAsync(testEvent2, CancellationToken.None), Times.Once);

            _mockConsumer2.Verify(x =>
                x.ConsumeAsync(testEvent1, CancellationToken.None), Times.Once);
            _mockConsumer2.Verify(x =>
                x.ConsumeAsync(testEvent2, CancellationToken.None), Times.Once);
        }
    }
}
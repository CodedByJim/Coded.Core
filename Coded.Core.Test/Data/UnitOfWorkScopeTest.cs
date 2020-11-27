using System.Threading;
using Coded.Core.Data;
using Coded.Core.Handler;
using Coded.Core.Testing;
using Moq;
using Xunit;

namespace Coded.Core.Test.Data
{
    public class UnitOfWorkScopeTest
    {
        private readonly Mock<IHandler<TestRequest, TestResponse>> _decoratedHandler;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly UnitOfWorkScope<TestRequest, TestResponse> _unitOfWorkScope;

        public UnitOfWorkScopeTest()
        {
            _decoratedHandler = new Mock<IHandler<TestRequest, TestResponse>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _unitOfWorkScope = new UnitOfWorkScope<TestRequest, TestResponse>(_decoratedHandler.Object, _unitOfWork.Object);
        }

        [Fact(DisplayName = "Unit of work scope handles, then commits, closes and disposes")]
        public void TestUnitOfWorkScope()
        {
            //Arrange
            _decoratedHandler.SetupHandle(x => new TestResponse());

            //Act
            var result = _unitOfWorkScope.Handle(new TestRequest(), CancellationToken.None);

            //Assert
            Assert.NotNull(result);
            _unitOfWork.Verify(x => x.Commit());
            _unitOfWork.Verify(x => x.Close());
            _unitOfWork.Verify(x => x.Dispose());
            _unitOfWork.VerifyNoOtherCalls();
        }
    }
}
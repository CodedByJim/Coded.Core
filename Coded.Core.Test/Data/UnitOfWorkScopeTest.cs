using System.Threading;
using System.Threading.Tasks;
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
            _decoratedHandler = new();
            _unitOfWork = new();
            _unitOfWorkScope = new(_decoratedHandler.Object, _unitOfWork.Object);
        }

        [Fact(DisplayName = "Unit of work scope handles, then commits, closes and disposes")]
        public void TestUnitOfWorkScope()
        {
            //Arrange
            _decoratedHandler.SetupHandle(_ => new());

            //Act
            Task<TestResponse?> result = _unitOfWorkScope.Handle(new(), CancellationToken.None);

            //Assert
            Assert.NotNull(result);
            _unitOfWork.Verify(x => x.Commit());
            _unitOfWork.Verify(x => x.Close());
            _unitOfWork.Verify(x => x.Dispose());
            _unitOfWork.VerifyNoOtherCalls();
        }
    }
}
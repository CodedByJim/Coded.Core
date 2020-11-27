using System.Threading;
using System.Threading.Tasks;
using Coded.Core.Handler;
using Coded.Core.Testing;
using Coded.Core.Validation;
using Moq;
using Xunit;

namespace Coded.Core.Test.Validation
{
    public class IoValidationHandlerDecoratorTest
    {
        private readonly Mock<IHandler<TestRequest, TestResponse>> _innerHandler;
        private readonly IoValidationHandlerDecorator<TestRequest, TestResponse> _ioValidation;
        private readonly Mock<IValidator<TestRequest>> _requestValidator;
        private readonly Mock<IValidator<TestResponse>> _responseValidator;

        public IoValidationHandlerDecoratorTest()
        {
            _innerHandler = new Mock<IHandler<TestRequest, TestResponse>>();
            _requestValidator = new Mock<IValidator<TestRequest>>();
            _responseValidator = new Mock<IValidator<TestResponse>>();
            _ioValidation = new IoValidationHandlerDecorator<TestRequest, TestResponse>(
                _innerHandler.Object,
                _requestValidator.Object,
                _responseValidator.Object);
        }

        [Fact(DisplayName = "Decorator validates input, calls inner handler and validates output.")]
        public async Task Decorator_ValidatesInputAndOutput()
        {
            //Arrange
            var response = new TestResponse();
            _innerHandler.SetupHandle(_ => response);

            var request = new TestRequest();

            //Act
            await _ioValidation.Handle(request, CancellationToken.None);

            //Assert
            _innerHandler.Verify(x => x.Handle(request, CancellationToken.None));
            _requestValidator.Verify(x => x.Validate(request));
            _responseValidator.Verify(x => x.Validate(response));
        }

        [Fact(DisplayName = "Decorator doesn't validate null values.")]
        public async Task Decorator_DoesntValidateNulls()
        {
            //Arrange
            _innerHandler.SetupHandle(_ => null);

            //Act
            await _ioValidation.Handle(null, CancellationToken.None);

            //Assert
            _innerHandler.Verify(x => x.Handle(null, CancellationToken.None));
            _requestValidator.VerifyNoOtherCalls();
            _responseValidator.VerifyNoOtherCalls();
        }
    }
}
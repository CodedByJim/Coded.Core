using System;
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
            _innerHandler = new();
            _requestValidator = new();
            _responseValidator = new();
            _ioValidation = new(
                _innerHandler.Object,
                _requestValidator.Object,
                _responseValidator.Object);
        }

        [Fact(DisplayName = "Decorator validates input, calls inner handler and validates output.")]
        public async Task Decorator_ValidatesInputAndOutput()
        {
            //Arrange
            TestResponse response = new();
            _innerHandler.SetupHandle(_ => response);

            TestRequest request = new();

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
            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _ioValidation.Handle(null!, CancellationToken.None)
            );

            //Assert
            _innerHandler.VerifyNoOtherCalls();
            _requestValidator.VerifyNoOtherCalls();
            _responseValidator.VerifyNoOtherCalls();
        }
    }
}
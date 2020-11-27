using System;
using System.Threading;
using System.Threading.Tasks;
using Coded.Core.Query;
using Coded.Core.Testing;
using Coded.Core.Validation;
using Moq;
using Xunit;

namespace Coded.Core.Test.Validation
{
    public class IoValidationQueryDecoratorTest
    {
        private readonly Mock<IQuery<TestRequest, TestResponse>> _innerQuery;
        private readonly IoValidationQueryDecorator<TestRequest, TestResponse> _ioValidation;
        private readonly Mock<IValidator<TestRequest>> _requestValidator;
        private readonly Mock<IValidator<TestResponse>> _responseValidator;

        public IoValidationQueryDecoratorTest()
        {
            _innerQuery = new Mock<IQuery<TestRequest, TestResponse>>();
            _requestValidator = new Mock<IValidator<TestRequest>>();
            _responseValidator = new Mock<IValidator<TestResponse>>();
            _ioValidation = new IoValidationQueryDecorator<TestRequest, TestResponse>(
                _innerQuery.Object,
                _requestValidator.Object,
                _responseValidator.Object);
        }

        [Fact(DisplayName = "Decorator validates input, calls inner handler and validates output.")]
        public async Task Decorator_ValidatesInputAndOutput()
        {
            //Arrange
            var result = new TestResponse();
            _innerQuery.SetupQuery(_ => result);

            var queryArgs = new TestRequest();

            //Act
            await _ioValidation.Query(queryArgs, CancellationToken.None);

            //Assert
            _innerQuery.Verify(x => x.Query(queryArgs, CancellationToken.None));
            _requestValidator.Verify(x => x.Validate(queryArgs));
            _responseValidator.Verify(x => x.Validate(result));
        }

        [Fact(DisplayName = "Decorator doesn't validate null outputs.")]
        public async Task Decorator_DoesntValidateNulls()
        {
            //Arrange
            var args = new TestRequest();
            _innerQuery.SetupQuery(_ => null);

            //Act
            await _ioValidation.Query(args, CancellationToken.None);

            //Assert
            _innerQuery.Verify(x => x.Query(args, CancellationToken.None));
            _requestValidator.Verify(x => x.Validate(args));
            _responseValidator.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Decorator throws ArgumentNullException when input is null.")]
        public async Task Decorator_ThrowsWhenArgAreNull()
        {
            //Act, Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _ioValidation.Query(null, CancellationToken.None)
            );
        }
    }
}
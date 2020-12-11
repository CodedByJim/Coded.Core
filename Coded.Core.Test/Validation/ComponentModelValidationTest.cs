using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using Coded.Core.Validation;
using Xunit;

namespace Coded.Core.Test.Validation
{
    public class ComponentModelValidationTest
    {
        private readonly ComponentModelValidator<TestRecord> _componentModelValidator = new();

        [Fact(DisplayName = "Correct input doesn't throw.")]
        public void ValidInput_Does_Not_Throw()
        {
            //Arrange
            TestRecord rootObject = new()
            {
                Id = 1,
                Value = "test"
            };

            //Act, Assert
            _componentModelValidator.Validate(rootObject);
        }

        [Fact(DisplayName = "Incorrect input throws.")]
        public void InvalidInput_Throws()
        {
            //Arrange
            TestRecord rootObject = new()
            {
                Id = 1000,
                Value = null
            };

            //Act, Assert
            Assert.Throws<ValidationException>(
                () => { _componentModelValidator.Validate(rootObject); });
        }

        [Fact(DisplayName = "Nested object gets validated.")]
        public void NestedObject_GetValidated()
        {
            //Arrange
            TestRecord rootObject = new()
            {
                Id = 100,
                Value = "abc",
                NestedObject = new()
                {
                    Id = 1000,
                    Value = null
                }
            };

            //Act, Assert
            Assert.Throws<ValidationException>(
                () => _componentModelValidator.Validate(rootObject));
        }

        [Fact(DisplayName = "Nested IEnumerable items get validated.")]
        public void NestedEnumerable_GetValidated()
        {
            //Arrange
            TestRecord rootObject = new()
            {
                Id = 100,
                Value = "abc",
                NestedObjects = new[]
                {
                    new TestRecord
                    {
                        Id = 1000,
                        Value = null
                    }
                }
            };

            //Act Assert
            Assert.Throws<ValidationException>(() => { _componentModelValidator.Validate(rootObject); });
        }

        [Fact(DisplayName = "Validator handles looped references.")]
        public void Loops_GetValidated()
        {
            //Arrange
            TestRecord rootObject = new()
            {
                Id = 100,
                Value = "abc",
                NestedObjects = new[]
                {
                    new TestRecord
                    {
                        Id = 100,
                        Value = "abc"
                    }
                }
            };
            rootObject.NestedObject = rootObject.NestedObjects[0];
            rootObject.NestedObjects[0].NestedObject = rootObject;

            //Act
            ConcurrentQueue<TestRecord> validationQueue = new();
            validationQueue.Enqueue(rootObject);

            _componentModelValidator.Validate(rootObject);
        }

        [Fact(DisplayName = "Validator handles null input.")]
        public void Null_GetsValidated()
        {
            _componentModelValidator.Validate(null!);
        }
    }
}
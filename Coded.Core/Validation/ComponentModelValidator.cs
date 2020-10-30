using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Coded.Core.Validation
{
    /// <summary>
    ///     Validates object with the component model validator
    /// </summary>
    /// <typeparam name="T">The type to validate</typeparam>
    public class ComponentModelValidator<T> : IValidator<T>
    {
        /// <inheritdoc />
        [DebuggerStepThrough]
        public void Validate(T instance)
        {
            var context = new ValidationContext(instance);
            Validator.ValidateObject(instance, context, true);
        }
    }
}
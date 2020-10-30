namespace Coded.Core.Validation
{
    /// <summary>
    ///     Validator for type <typeparamref name="T" />
    /// </summary>
    /// <typeparam name="T">The type to validate.</typeparam>
    public interface IValidator<in T>
    {
        /// <summary>
        ///     Validate the instance
        /// </summary>
        /// <param name="instance">The instance to validate</param>
        void Validate(T instance);
    }
}
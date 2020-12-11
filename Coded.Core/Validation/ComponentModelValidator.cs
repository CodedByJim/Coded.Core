using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

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
        public void Validate(T rootObject)
        {
            if (rootObject == null) return;
            ConcurrentQueue<object> validationQueue = new();
            validationQueue.Enqueue(rootObject);
            List<object> known = new();
            while (validationQueue.TryDequeue(out var objectToValidate))
                ValidateItem(objectToValidate, validationQueue, known);
        }

        private static void ValidateItem(object objectToValidate, ConcurrentQueue<object> validationQueue, ICollection<object> known)
        {
            PreprocessObject(objectToValidate, validationQueue, known, out var shouldBeValidated);

            if (!shouldBeValidated)
                return;

            //Do the actual validation.
            Validator.ValidateObject(objectToValidate,
                new(objectToValidate),
                true);
            ValidateNestedItems(objectToValidate, validationQueue);
        }

        private static void PreprocessObject(
            object objectToValidate,
            ConcurrentQueue<object> validationQueue,
            ICollection<object> known,
            out bool shouldBeValidated)
        {
            shouldBeValidated = true;

            if (known.Any(knownItem => knownItem == objectToValidate))
            {
                shouldBeValidated = false;
                return;
            }

            known.Add(objectToValidate);

            var objectType = objectToValidate.GetType();
            var underlyingType = Nullable.GetUnderlyingType(objectType) ?? objectType;

            if (objectToValidate is string || underlyingType.IsPrimitive || underlyingType.IsValueType)
            {
                shouldBeValidated = false;
            }
            else if (objectToValidate is IEnumerable list)
            {
                shouldBeValidated = false;
                foreach (var listItem in list)
                    if (listItem != null)
                        validationQueue.Enqueue(listItem);
            }
        }

        private static void ValidateNestedItems(object objectToValidate, ConcurrentQueue<object> validationQueue)
        {
            var properties = objectToValidate.GetType().GetProperties()
                .Where(prop => prop.CanRead);

            foreach (var property in properties)
            {
                var nestedValue = property.GetValue(objectToValidate);
                if (nestedValue != null)
                    validationQueue.Enqueue(nestedValue);
            }
        }
    }
}
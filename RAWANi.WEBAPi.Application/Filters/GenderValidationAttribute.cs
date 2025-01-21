using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Filters
{
    public class GenderValidationAttribute : ValidationAttribute
    {
        private readonly string[] _allowedValues = { "Male", "Female" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Gender cannot be null or empty.");
            }

            // Convert the input value to lowercase for case-insensitive comparison
            var inputValue = value.ToString().Trim();

            // Check if the input value matches any of the allowed values (case-insensitive)
            if (!_allowedValues.Any(allowedValue => allowedValue.Equals(inputValue, StringComparison.OrdinalIgnoreCase)))
            {
                return new ValidationResult(
                    $"Gender must be one of the following: {string.Join(", ", _allowedValues)}.");
            }

            return ValidationResult.Success;
        }
    }
}

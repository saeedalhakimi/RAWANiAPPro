using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Filters
{
    public class AgeRangeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;
        private readonly int _maximumAge;

        public AgeRangeAttribute(int minimumAge, int maximumAge)
        {
            _minimumAge = minimumAge;
            _maximumAge = maximumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                var today = DateTime.UtcNow;
                var age = today.Year - dateOfBirth.Year;

                // Adjust for leap years and exact birthdate
                if (dateOfBirth.Date > today.AddYears(-age))
                    age--;

                if (age < _minimumAge || age > _maximumAge)
                    return new ValidationResult($"Age must be between {_minimumAge} and {_maximumAge} years.");

                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid date of birth.");
        }
    }
}

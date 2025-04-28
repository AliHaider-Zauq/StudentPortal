using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StudentPortal.Validators
{
    public class ValidateZauqEmailAttribute : ValidationAttribute
    {
        private readonly string _allowedDomain = "zauq.com";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string email && email.Contains("@"))
            {
                var domain = email.Split('@').Last();
                if (domain.ToLower() == _allowedDomain.ToLower())
                {
                    return ValidationResult.Success; // Email is valid
                }
            }

            return new ValidationResult($"Email must be from '{_allowedDomain}'.");
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Isbn13Attribute : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("ISBN cant be null.");
            }
            string isbn = value.ToString()!;
            isbn = isbn.Replace("-", "").Trim();
            bool isOnlyDigits = Regex.IsMatch(isbn, @"^\d+$");
            if (!isOnlyDigits)
            {
                return new ValidationResult("ISBN can contain only digits and dashes.");
            }
            else if (isbn.Length != 13)
            {
                return new ValidationResult("ISBN must contain 13 digits.");
            }

            return ValidationResult.Success;
        }
    }
}

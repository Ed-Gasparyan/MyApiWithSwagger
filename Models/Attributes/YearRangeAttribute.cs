using System.ComponentModel.DataAnnotations;

namespace Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class YearRangeAttribute :ValidationAttribute
    {
        private readonly int _minYear;

        public YearRangeAttribute(int year)
        {
            _minYear = year;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (!int.TryParse(value.ToString(), out int year))
            {
                return new ValidationResult("Published year must be a number.");
            }
            if (year < _minYear || year > DateTime.Now.Year)
            {
                return new ValidationResult("Invalid year of publication.");
            }

            return ValidationResult.Success;
        }
    }
}

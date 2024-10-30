using System.ComponentModel.DataAnnotations;

namespace LavenderChessClock1.Validators
{
    public class IncrementLengthValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Value for \"increment length\" cannot be null.", new[] { validationContext.MemberName });
            }

            int valueInt = (int)value;

            if (!int.TryParse(value.ToString(), out valueInt) || valueInt < 0)
            {
                return new ValidationResult("Value for \"increment length\" must be a positive integer, or 0.", new[] { validationContext.MemberName });
            }
            else if (valueInt > 7200)
            {
                return new ValidationResult("Value for \"increment length\" must be less than or equal to 7200 (2 hours). ", new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}

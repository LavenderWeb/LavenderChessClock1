using LavenderChessClock1.Chess;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LavenderChessClock1.Validators
{
    public class SecondsValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Value for \"seconds\" cannot be null.", new[] { validationContext.MemberName });
            }

            int valueInt = (int)value;

            if (!int.TryParse(value.ToString(), out valueInt) || valueInt < 1)
            {
                return new ValidationResult("Value for \"seconds\" must be a positive integer.", new[] { validationContext.MemberName });
            }
            else if (valueInt > 7200)
            {
                return new ValidationResult("Value for \"seconds\" must be less than or equal to 7200 (2 hours). ", new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}

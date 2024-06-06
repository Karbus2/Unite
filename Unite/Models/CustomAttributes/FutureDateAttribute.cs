using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Unite.Models.CustomAttributes
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.GetType() == typeof(DateTime))
            {
                DateTime dateTime = (DateTime)value;
                if (dateTime > DateTime.Now)
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                throw new Exception("Walidowana wartość musi być typu DateTime");
            }
            return new ValidationResult($"Data musi być przyszła.");
        }
    }
}

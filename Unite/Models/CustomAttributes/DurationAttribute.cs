using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Unite.Models.CustomAttributes
{
    public class DurationAttribute : ValidationAttribute
    {
        public string OtherProperty { get; set; }
        public double Days { get; set; }
        public DurationAttribute(string otherProperty, double days)
        {
            OtherProperty = otherProperty;
            Days = days;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherValueInfo = validationContext.ObjectType.GetRuntimeProperty(OtherProperty);
            if (otherValueInfo == null)
            {
                return new ValidationResult("Taka właściwość nie istnieje");
            }
            if (otherValueInfo.GetIndexParameters().Length > 0)
            {
                throw new ArgumentException("Nie znaleziono parametru o podanej nazwie");
            }

            object? _otherValue = otherValueInfo.GetValue(validationContext.ObjectInstance, null);

            if (_otherValue == null)
            {
                return new ValidationResult($"Wartość nie może być pusta.");
            }
            if (value.GetType() == typeof(DateTime) && _otherValue.GetType() == typeof(DateTime))
            {
                DateTime endOfPeriod = (DateTime)value;
                if (endOfPeriod.AddDays(-Days) <= (DateTime)_otherValue)
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                throw new Exception("One of the properties is not DateTime");
            }
            return new ValidationResult($"Maksymalny czas trwania {Days} dni od {GetDisplayNameForProperty(otherValueInfo)}");
        }
        private string? GetDisplayNameForProperty(PropertyInfo property)
        {
            IEnumerable<Attribute> attributes = CustomAttributeExtensions.GetCustomAttributes(property, true);
            foreach (Attribute attribute in attributes)
            {
                if (attribute is DisplayAttribute display)
                {
                    return display.GetName();
                }
            }

            return OtherProperty;
        }
    }
}

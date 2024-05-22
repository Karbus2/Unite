using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Unite.Models.CustomAttributes
{
    public class IsHigherAttribute<T> : ValidationAttribute where T : IComparable
    {
        public string OtherProperty { get; set; }
        public IsHigherAttribute(string otherProperty)
        {
            OtherProperty = otherProperty;
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
            if (value.GetType() == typeof(T) && _otherValue.GetType() == typeof(T))
            {
                T higerValue = (T)value;
                if (higerValue.CompareTo(_otherValue) > 0)
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                throw new Exception("Nieprawidłowy typ");
            }
            return new ValidationResult($"Wartość nie może być mniejsza niż {GetDisplayNameForProperty(otherValueInfo)}");
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

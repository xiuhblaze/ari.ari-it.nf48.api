using System;
using System.ComponentModel.DataAnnotations;

namespace Arysoft.ARI.NF48.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ValidEnumValue : ValidationAttribute
    {
        private readonly Type _enumType;

        public ValidEnumValue(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum) throw new ArgumentException("Type must be an enum.", nameof(enumType));
            _enumType = enumType;
        }

        public override bool IsValid(object value)
        {
            // null is considered invalid: use [Required] if you want explicit message, o bien devolver false aquí
            if (value == null) return false;

            // If value is already an enum or underlying numeric, try Enum.IsDefined
            if (Enum.IsDefined(_enumType, value)) return true;

            // Try to convert numeric types to the enum underlying type
            try
            {
                var underlyingType = Enum.GetUnderlyingType(_enumType);
                var converted = Convert.ChangeType(value, underlyingType);
                return Enum.IsDefined(_enumType, converted);
            }
            catch
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return string.IsNullOrEmpty(ErrorMessage)
                ? $"{name} must be a valid {_enumType.Name} value."
                : ErrorMessage;
        }
    }
}
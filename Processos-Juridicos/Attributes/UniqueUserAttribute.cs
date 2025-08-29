using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class UniqueUserAttribute : ValidationAttribute
{

    private const string _messageKey = "FieldMustBeUnique";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext context
            || validationContext.ObjectInstance is not UserDto userDto
            || value == null)
        {
            return ValidationResult.Success;
        }

        var userNii = value as string;

        if (!string.IsNullOrEmpty(userDto.OriginalUserNii) &&
            string.Equals(userDto.OriginalUserNii, userNii, StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Success;
        }

        var exists = context.Users.Any(p => p.UserNii == userNii);

        return exists
            ? new ValidationResult(GlobalTextManager.GetString(_messageKey))
            : ValidationResult.Success;
    }

}

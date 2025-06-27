using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]


public sealed class UniqueProcessNiupmAttribute : ValidationAttribute
{
    private const string _messageKey = "FieldMustBeUnique";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext context || validationContext.ObjectInstance is not ProcessDto processDto || value == null)
        {
            return ValidationResult.Success;
        }

        var processNuipm = value as string;

        var existingType = context.Processes
            .Any(m => m.Nuipm == processNuipm && m.ProcessId != processDto.ProcessId);

        return existingType
            ? new ValidationResult(GlobalTextManager.GetString(_messageKey))
            : ValidationResult.Success;
    }
}

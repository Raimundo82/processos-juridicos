using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UniqueHarmedOrCasualtyNameAttribute : ValidationAttribute
{
    private const string _messageKey = "FieldMustBeUnique";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext context || validationContext.ObjectInstance is not HarmedOrCasualtyDto casualtyDto || value == null)
        {
            return ValidationResult.Success;
        }

        var casualtyName = value as string;

        var existingType = context.HarmedOrCasualties
            .Any(p => p.CasualtyName == casualtyName && p.CasualtyId != casualtyDto.CasualtyId);

        return existingType ? new ValidationResult(GlobalTextManager.GetString(_messageKey)) : ValidationResult.Success;
    }
}

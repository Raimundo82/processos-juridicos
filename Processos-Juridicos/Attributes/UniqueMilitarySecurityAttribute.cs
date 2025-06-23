using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]


public sealed class UniqueMilitarySecurityAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext context || validationContext.ObjectInstance is not MilitarySecurityDto militarySecurityDto || value == null)
        {
            return ValidationResult.Success;
        }

        var militarySecurityName = value as string;

        var existingType = context.Military_securities
            .Any(m => m.MilitarySecurityName == militarySecurityName && m.MilitarySecurityId != militarySecurityDto.MilitarySecurityId);

        return existingType
            ? new ValidationResult($"Já existe uma Segurança Militar com o nome '{militarySecurityName}'.")
            : ValidationResult.Success;
    }
}

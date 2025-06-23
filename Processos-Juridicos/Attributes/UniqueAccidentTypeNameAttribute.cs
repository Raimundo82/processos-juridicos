using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class UniqueAccidentTypeNameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext context || validationContext.ObjectInstance is not AccidentTypeDto accidentTypeDto || value == null)
        {
            return ValidationResult.Success;
        }

        var accidentTypeName = value as string;

        var existingType = context.Accident_types
            .Any(p => p.AccidentTypeName == accidentTypeName && p.AccidentTypeId != accidentTypeDto.AccidentTypeId);

        return existingType
            ? new ValidationResult($"Já existe um Tipo de Acidente com o nome '{accidentTypeName}'.")
            : ValidationResult.Success;
    }

}

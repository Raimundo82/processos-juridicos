using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class UniqueProcessTypeNameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext context || validationContext.ObjectInstance is not ProcessTypeDto processTypeDto || value == null)
        {
            return ValidationResult.Success;
        }

        var processTypeName = value as string;

        var existingType = context.Process_types
            .Any(p => p.ProcessTypeName == processTypeName && p.ProcessTypeId != processTypeDto.ProcessTypeId);

        return existingType
            ? new ValidationResult($"Já existe um Tipo de Processo com o nome '{processTypeName}'.")
            : ValidationResult.Success;
    }
}

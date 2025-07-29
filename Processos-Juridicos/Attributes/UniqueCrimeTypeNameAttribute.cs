using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UniqueCrimeTypeNameAttribute : ValidationAttribute
{
    private const string _messageKey = "FieldMustBeUnique";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext context || validationContext.ObjectInstance is not CrimeTypeDto crimeTypeDto || value == null)
        {
            return ValidationResult.Success;
        }

        var crimeTypeName = value as string;

        var existingType = context.CrimeTypes
            .Any(p => p.CrimeTypeName == crimeTypeName && p.CrimeTypeId != crimeTypeDto.CrimeTypeId);

        return existingType ? new ValidationResult(GlobalTextManager.GetString(_messageKey)) : ValidationResult.Success;
    }
}

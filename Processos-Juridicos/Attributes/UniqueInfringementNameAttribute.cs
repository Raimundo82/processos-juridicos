using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UniqueInfringementNameAttribute : ValidationAttribute
{
    private const string _messageKey = "FieldMustBeUnique";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext context || validationContext.ObjectInstance is not InfringementDto infringementDto || value == null)
        {
            return ValidationResult.Success;
        }

        var infringementName = value as string;

        var existingType = context.Infringements
            .Any(p => p.InfringementName == infringementName && p.InfringementId != infringementDto.InfringementId);

        return existingType ? new ValidationResult(GlobalTextManager.GetString(_messageKey)) : ValidationResult.Success;
    }
}

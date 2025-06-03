using System.ComponentModel.DataAnnotations;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class UniqueAccidentTypeNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            var accidentTypeDto = validationContext.ObjectInstance as AccidentTypeDto;

            if (context == null || accidentTypeDto == null || value == null)
            {
                return ValidationResult.Success;
            }

            var accidentTypeName = value as string;

            var existingType = context.Accident_types
                .Any(p => p.AccidentTypeName == accidentTypeName && p.AccidentTypeId != accidentTypeDto.AccidentTypeId);

            if (existingType)
            {
                return new ValidationResult($"Já existe um Tipo de Acidente com o nome '{accidentTypeName}'.");
            }

            return ValidationResult.Success;
        }

    }
}

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Processos_Juridicos.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class UniqueProcessTypeNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            var processTypeDto = validationContext.ObjectInstance as ProcessTypeDto;

            if (context == null || processTypeDto == null || value == null)
            {
                return ValidationResult.Success;
            }

            var processTypeName = value as string;

            var existingType = context.Process_types
                .Any(p => p.ProcessTypeName == processTypeName && p.ProcessTypeId != processTypeDto.ProcessTypeId);

            if (existingType)
            {
                return new ValidationResult($"Já existe um Tipo de Processo com o nome '{processTypeName}'.");
            }

            return ValidationResult.Success;
        }
    }
}

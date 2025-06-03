using System.ComponentModel.DataAnnotations;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]


    public sealed class UniqueMilitarySecurityAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            var militarySecurityDto = validationContext.ObjectInstance as MilitarySecurityDto;

            if (context == null || militarySecurityDto == null || value == null)
            {
                return ValidationResult.Success;
            }

            var militarySecurityName = value as string;

            var existingType = context.Military_securities
                .Any(m => m.MilitarySecurityName== militarySecurityName && m.MilitarySecurityId != militarySecurityDto.MilitarySecurityId);

            if (existingType)
            {
                return new ValidationResult($"Já existe uma Segurança Militar com o nome '{militarySecurityName}'.");
            }

            return ValidationResult.Success;
        }
    }

}

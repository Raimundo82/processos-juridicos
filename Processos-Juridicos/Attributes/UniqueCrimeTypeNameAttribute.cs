using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Processos_Juridicos.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UniqueCrimeTypeNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            var crimeTypeDto = validationContext.ObjectInstance as CrimeTypeDto;

            if (context == null || crimeTypeDto == null || value == null)
            {
                return ValidationResult.Success;
            }

            var crimeTypeName = value as string;

            var existingType = context.Crime_types
                .Any(p => p.CrimeTypeName == crimeTypeName && p.CrimeTypeId != crimeTypeDto.CrimeTypeId);

            if (existingType)
            {
                return new ValidationResult($"Já existe um Tipo de Crime com o nome '{crimeTypeName}'.");
            }

            return ValidationResult.Success;
        }
    }
}

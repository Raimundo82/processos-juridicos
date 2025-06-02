using System.ComponentModel.DataAnnotations;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.Attributes
{
    public abstract class UniqueUnitAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly string _displayName;

        protected UniqueUnitAttribute(string propertyName, string displayName)
        {
            _propertyName = propertyName;
            _displayName = displayName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var context = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            var unitDto = validationContext.ObjectInstance as UnitDto;

            if (context == null || unitDto == null || value == null)
            {
                return ValidationResult.Success;
            }

            var existingType = context.Units
                .Any(p => EF.Property<string>(p, _propertyName) == value as string && p.UnitId != unitDto.UnitId);

            if (existingType)
            {
                return new ValidationResult($"Já existe uma Unidade com o {_displayName} '{value}'.");
            }

            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UniqueUnitNameAttribute : UniqueUnitAttribute
    {
        public UniqueUnitNameAttribute() : base("UnitName", "nome") { }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UniqueUnitCodeAttribute : UniqueUnitAttribute
    {
        public UniqueUnitCodeAttribute() : base("UnitCode", "código") { }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UniqueUnitAcronymAttribute : UniqueUnitAttribute
    {
        public UniqueUnitAcronymAttribute() : base("UnitAcronym", "acrônimo") { }
    }
}
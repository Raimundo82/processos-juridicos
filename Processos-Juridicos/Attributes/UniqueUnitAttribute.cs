using System.ComponentModel.DataAnnotations;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

public abstract class UniqueUnitAttribute(string propertyName) : ValidationAttribute
{
    private readonly string _propertyName = propertyName;
    private const string _messageKey = "FieldMustBeUnique";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext context || validationContext.ObjectInstance is not UnitDto unitDto || value == null)
        {
            return ValidationResult.Success;
        }

        var existingType = context.Units
            .Any(p => EF.Property<string>(p, _propertyName) == (value as string) && p.UnitId != unitDto.UnitId);

        return existingType ? new ValidationResult(GlobalTextManager.GetString(_messageKey)) : ValidationResult.Success;
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UniqueUnitNameAttribute : UniqueUnitAttribute
{
    public UniqueUnitNameAttribute() : base("UnitName") { }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UniqueUnitCodeAttribute : UniqueUnitAttribute
{
    public UniqueUnitCodeAttribute() : base("UnitCode") { }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UniqueUnitAcronymAttribute : UniqueUnitAttribute
{
    public UniqueUnitAcronymAttribute() : base("UnitAcronym") { }
}

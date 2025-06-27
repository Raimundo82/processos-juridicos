using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    AllowMultiple = false,
    Inherited = true)]
public class FutureDateAttribute : ValidationAttribute
{
    private static readonly string _message =
        GlobalTextManager.GetString("DateMustBeTodayOrLater");

    public FutureDateAttribute()
        : base(_message)
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var dateTime = new DateOnly();
        DateOnly insertedDate = dateTime;

        if (value == null)
        {
            return ValidationResult.Success;
        }
        else if (value is DateOnly dt)
        {
            insertedDate = dt;
        }

        return insertedDate < DateOnly.FromDateTime(DateTime.Now)
            ? new ValidationResult(
                ErrorMessage,
                [validationContext.MemberName!]
            )
            : ValidationResult.Success;
    }
}

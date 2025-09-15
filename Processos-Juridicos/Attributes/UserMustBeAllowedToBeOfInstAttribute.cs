using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Data;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UserMustBeAllowedToBeOfInstAttribute : ValidationAttribute
{

    private readonly string _messageKey = "UserCannotBeOfInst";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        var raw = value.ToString()?.Trim();
        if (string.IsNullOrEmpty(raw))
        {
            return ValidationResult.Success;
        }

        var employeeId = value.ToString();

        var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
        return !dbContext!.Users.Any(u => u.UserNii == employeeId)
            ? new ValidationResult(ErrorMessage ?? GlobalTextManager.GetString(_messageKey))
            : ValidationResult.Success;
    }
}

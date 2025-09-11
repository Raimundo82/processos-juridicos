using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Data;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UserMustExistInDatabaseAttribute : ValidationAttribute
{
    private readonly string _messageKey = "UserDoesNotExistMessage";
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IEnumerable<string> ids || !ids.Any() || value == null)
        {
            return ValidationResult.Success;
        }

        if (validationContext.GetService(typeof(AppDbContext)) is not AppDbContext dbContext)
        {
            throw new InvalidOperationException("AppDbContext could not be resolved from ValidationContext.");
        }

        // Find any IDs that don't exist in the Users table
        var invalidIds = ids
            .Where(id => !dbContext.Users.Any(u => u.UserNii == id))
            .ToList();

        return invalidIds.Count > 0
            ? new ValidationResult(ErrorMessage ?? GlobalTextManager.GetString(_messageKey))
            : ValidationResult.Success;
    }


}

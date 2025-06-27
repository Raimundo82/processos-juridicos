using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    AllowMultiple = false,
    Inherited = true)]
public class EntityFieldIsRequiredAttribute(string fieldDisplayName) : RequiredAttribute
{
    private readonly string _messageKey = "MandatoryFieldMessage";
    private readonly string _fieldDisplayName = fieldDisplayName;

    public override string FormatErrorMessage(string name)
    {
        var template = GlobalTextManager.GetString(_messageKey);
        return string.Format(template, _fieldDisplayName);
    }
}

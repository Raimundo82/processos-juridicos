using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    AllowMultiple = false,
    Inherited = true)]
public class MaxLengthAttribute(int maximumLength, string fieldName) : StringLengthAttribute(maximumLength)
{
    private readonly string _messageKey = "MaximumFieldSizeExceededMessage";
    private readonly int _maximumLength = maximumLength;
    private readonly string _fieldName = fieldName;
    public override string FormatErrorMessage(string name)
    {
        var template = GlobalTextManager.GetString(_messageKey);
        return string.Format(template, _fieldName, _maximumLength);
    }
}

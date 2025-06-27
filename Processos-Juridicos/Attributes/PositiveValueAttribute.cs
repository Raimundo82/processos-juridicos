using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Attributes;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field,
    AllowMultiple = false,
    Inherited = true)]
public class PositiveValueAttribute(string fieldName) : RangeAttribute(1, double.MaxValue)
{
    private readonly string _key = "MustBeAPositiveNumberMessage";

    public override string FormatErrorMessage(string name)
    {
        var template = GlobalTextManager.GetString(_key);
        return string.Format(template, fieldName);
    }
}

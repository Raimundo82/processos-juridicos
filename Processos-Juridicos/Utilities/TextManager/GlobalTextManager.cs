using Processos_Juridicos.Utilities.TextManager.Interfaces;

namespace Processos_Juridicos.Utilities.TextManager;
public static class GlobalTextManager
{
    private static IJsonTextManager? _resourceManager;

    public static void SetManager(IJsonTextManager manager)
    {
        _resourceManager = manager;
    }

    public static string GetString(string key)
    {
        return _resourceManager?.GetString(key) ?? $"[[{key}]]";
    }

}

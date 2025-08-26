using AngleSharp;
using AngleSharp.Dom;

namespace Processos_Juridicos.Tests;

public static class AngleSharpHelper
{
    private static readonly IConfiguration _config = Configuration.Default;

    public static async Task<IDocument> GetDocumentAsync(
        this HttpClient client,
        string relativeUrl)
    {
        var html = await client.GetStringAsync(relativeUrl);
        return await BrowsingContext
                     .New(_config)
                     .OpenAsync(req => req.Content(html));
    }
}

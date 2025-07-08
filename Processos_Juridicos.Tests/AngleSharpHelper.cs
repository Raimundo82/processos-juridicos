// put this file under your test project (e.g. Tests/Helpers/AngleSharpHelpers.cs)
using AngleSharp;
using AngleSharp.Dom;

namespace Processos_Juridicos.Tests;

public static class AngleSharpHelper
{
    // configuration can be shared
    private static readonly IConfiguration _config = Configuration.Default;

    // the 'this HttpClient' makes it an extension
    public static async Task<IDocument> GetDocumentAsync(
        this HttpClient client,
        string relativeUrl)
    {
        // you might need to prepend BaseAddress if you set it on the client
        var html = await client.GetStringAsync(relativeUrl);
        return await BrowsingContext
                     .New(_config)
                     .OpenAsync(req => req.Content(html));
    }
}

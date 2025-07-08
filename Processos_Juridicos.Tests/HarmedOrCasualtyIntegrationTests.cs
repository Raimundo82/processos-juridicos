using System.Net;

using AngleSharp;
using AngleSharp.Dom;

using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests;

public class HarmedOrCasualtyIntegrationTests
{
    [Fact]
    public async Task Get_HarmedOrCasualtyList_ReturnsHarmedOrCasualtyList()
    {

        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Harmed_or_casualties.AddRange(
            new HarmedOrCasualty { CasualtyName = "Ferido" },
            new HarmedOrCasualty { CasualtyName = "Outros" }
        );
        _ = await db.SaveChangesAsync();

        using HttpClient client = factory.CreateClient();

        //Act
        IDocument doc = await client.GetDocumentAsync("/HarmedOrCasualty/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Ferido", cellTexts);
        Assert.Contains("Outros", cellTexts);
    }

    [Fact]
    public async Task List_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using HttpClient client = factory.CreateClient();

        // Act
        IDocument doc = await client.GetDocumentAsync("/HarmedOrCasualty/List");

        // Assert
        IHtmlCollection<IElement> rows = doc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);

        IHtmlCollection<IElement> cells = doc.QuerySelectorAll("table tbody td");
        Assert.Empty(cells);
    }

    [Fact]
    public async Task Create_Post_Valid_RedirectsAndPersists()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using HttpClient client = factory.CreateClient();

        var fields = new Dictionary<string, string>
        {
            ["CasualtyName"] = "Ferido"
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        HttpResponseMessage postResponse = await client.PostAsync("/HarmedOrCasualty/Create", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        IDocument listDoc = await client.GetDocumentAsync("/HarmedOrCasualty/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td")
                             .Select(td => td.TextContent.Trim());
        Assert.Contains("Ferido", names);
    }
    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsEmptyList()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using HttpClient client = factory.CreateClient();

        IDocument getDoc = await client.GetDocumentAsync("/HarmedOrCasualty/Create");
        IElement form = getDoc.QuerySelector("form[action='/HarmedOrCasualty/Create']")!;
        var action = form.GetAttribute("action")!;  // "/HarmedOrCasualty/Create"

        var fields = new Dictionary<string, string>
        {
            ["CasualtyName"] = string.Empty
        };

        //Act
        HttpResponseMessage postResponse = await client.PostAsync(
            action,
            new FormUrlEncodedContent(fields)
        );

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await client.GetDocumentAsync("/HarmedOrCasualty/List");

        IHtmlCollection<IElement> rows = listDoc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);

        IHtmlCollection<IElement> cells = listDoc.QuerySelectorAll("table tbody td");
        Assert.Empty(cells);


        var html = await postResponse.Content.ReadAsStringAsync();
        IDocument errDoc = await BrowsingContext
                        .New(Configuration.Default)
                        .OpenAsync(req => req.Content(html));
        Assert.Contains(
            "O campo Nome da Categoria de ferido é obrigatório.",
            errDoc.DocumentElement.TextContent,
            StringComparison.OrdinalIgnoreCase
        );
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var HarmedOrCasualty = new HarmedOrCasualty
        {
            CasualtyName = "Ferido"
        };

        _ = db.Harmed_or_casualties.Add(HarmedOrCasualty);

        _ = await db.SaveChangesAsync();

        var id = HarmedOrCasualty.CasualtyId;

        using HttpClient client = factory.CreateClient();

        // Act
        IDocument doc = await client.GetDocumentAsync($"/HarmedOrCasualty/Edit/{id}");

        // Assert
        IElement? form = doc.QuerySelector("form[action^='/HarmedOrCasualty/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=CasualtyId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=CasualtyName]")!;
        Assert.Equal("Ferido", nameInput.GetAttribute("value"));
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var HarmedOrCasualty = new HarmedOrCasualty
        {
            CasualtyName = "Ferido"
        };

        _ = db.Harmed_or_casualties.Add(HarmedOrCasualty);

        _ = await db.SaveChangesAsync();

        var id = HarmedOrCasualty.CasualtyId.ToString()!;

        using HttpClient client = factory.CreateClient();
        IDocument editDoc = await client.GetDocumentAsync($"/HarmedOrCasualty/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/HarmedOrCasualty/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string>
        {
            ["CasualtyId"] = id.ToString(),
            ["CasualtyName"] = "Atualizado",
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await client.PostAsync(action, content);

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await client.GetDocumentAsync("/HarmedOrCasualty/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td")
                             .Select(td => td.TextContent.Trim());
        Assert.Contains("Atualizado", names);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var HarmedOrCasualty = new HarmedOrCasualty
        {
            CasualtyName = "Ferido"
        };

        _ = db.Harmed_or_casualties.Add(HarmedOrCasualty);

        _ = await db.SaveChangesAsync();

        var id = HarmedOrCasualty.CasualtyId.ToString()!;

        using HttpClient client = factory.CreateClient();

        IDocument editDoc = await client.GetDocumentAsync($"/HarmedOrCasualty/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/HarmedOrCasualty/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string>
        {
            ["CasualtyId"] = id.ToString(),
            ["HarmedOrCasualtyName"] = string.Empty
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        //Act
        IDocument doc = await client.GetDocumentAsync("/HarmedOrCasualty/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Ferido", cellTexts);

        var html = await postResponse.Content.ReadAsStringAsync();
        IDocument errDoc = await BrowsingContext
                        .New(Configuration.Default)
                        .OpenAsync(req => req.Content(html));
        Assert.Contains(
            "O campo Nome da Categoria de ferido é obrigatório.",
            errDoc.DocumentElement.TextContent,
            StringComparison.OrdinalIgnoreCase
        );
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var HarmedOrCasualty = new HarmedOrCasualty
        {
            CasualtyName = "Ferido"
        };

        _ = db.Harmed_or_casualties.Add(HarmedOrCasualty);

        _ = await db.SaveChangesAsync();

        var id = HarmedOrCasualty.CasualtyId.ToString()!;

        using HttpClient client = factory.CreateClient();

        IDocument listDoc = await client.GetDocumentAsync("/HarmedOrCasualty/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/HarmedOrCasualty/Delete']")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        // 2) POST the form with the correct Id & token
        var fields = new Dictionary<string, string>
        {
            ["CasualtyId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await client.PostAsync("/HarmedOrCasualty/Delete/1", content);

        // Assert 
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await client.GetDocumentAsync("/HarmedOrCasualty/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.DoesNotContain("Ferido", names);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        using HttpClient client = factory.CreateClient();

        db.Harmed_or_casualties.AddRange(
           new HarmedOrCasualty { CasualtyName = "Ferido" }
       );
        _ = await db.SaveChangesAsync();

        IDocument listDoc = await client.GetDocumentAsync("/HarmedOrCasualty/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/HarmedOrCasualty/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["CasualtyId"] = "9999",
            ["__RequestVerificationToken"] = token
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await client.GetDocumentAsync("/HarmedOrCasualty/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.Contains("Ferido", names);
    }

}

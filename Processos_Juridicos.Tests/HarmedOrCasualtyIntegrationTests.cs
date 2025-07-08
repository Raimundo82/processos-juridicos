using System.Net;

using AngleSharp;
using AngleSharp.Dom;

using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests;

public class HarmedOrCasualtyIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();
    [Fact]
    public async Task Get_HarmedOrCasualtyList_ReturnsHarmedOrCasualtyList()

    {

        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Harmed_or_casualties.AddRange(
            new HarmedOrCasualty { CasualtyName = "Ferido" },
            new HarmedOrCasualty { CasualtyName = "Outros" }
        );
        _ = await dbContext.SaveChangesAsync();


        //Act
        IDocument doc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Ferido", cellTexts);
        Assert.Contains("Outros", cellTexts);

        await DbUtilities.RemoveEntitiesAsync<HarmedOrCasualty>(dbContext);
    }

    [Fact]
    public async Task List_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange

        // Act
        IDocument doc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");

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
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var fields = new Dictionary<string, string>
        {
            ["CasualtyName"] = "Ferido"
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        HttpResponseMessage postResponse = await _client.PostAsync("/HarmedOrCasualty/Create", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        IDocument listDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td")
                             .Select(td => td.TextContent.Trim());
        Assert.Contains("Ferido", names);
        await DbUtilities.RemoveEntitiesAsync<HarmedOrCasualty>(dbContext);
    }
    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsEmptyList()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        IDocument getDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/Create");
        IElement form = getDoc.QuerySelector("form[action='/HarmedOrCasualty/Create']")!;
        var action = form.GetAttribute("action")!;  // "/HarmedOrCasualty/Create"

        var fields = new Dictionary<string, string>
        {
            ["CasualtyName"] = string.Empty
        };

        //Act
        HttpResponseMessage postResponse = await _client.PostAsync(
            action,
            new FormUrlEncodedContent(fields)
        );

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");

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
        await DbUtilities.RemoveEntitiesAsync<HarmedOrCasualty>(dbContext);
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var HarmedOrCasualty = new HarmedOrCasualty
        {
            CasualtyName = "Ferido"
        };

        _ = dbContext.Harmed_or_casualties.Add(HarmedOrCasualty);

        _ = await dbContext.SaveChangesAsync();

        var id = HarmedOrCasualty.CasualtyId;


        // Act
        IDocument doc = await _client.GetDocumentAsync($"/HarmedOrCasualty/Edit/{id}");

        // Assert
        IElement? form = doc.QuerySelector("form[action^='/HarmedOrCasualty/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=CasualtyId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=CasualtyName]")!;
        Assert.Equal("Ferido", nameInput.GetAttribute("value"));

        await DbUtilities.RemoveEntitiesAsync<HarmedOrCasualty>(dbContext);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var HarmedOrCasualty = new HarmedOrCasualty
        {
            CasualtyName = "Ferido"
        };

        _ = dbContext.Harmed_or_casualties.Add(HarmedOrCasualty);

        _ = await dbContext.SaveChangesAsync();

        var id = HarmedOrCasualty.CasualtyId.ToString()!;

        IDocument editDoc = await _client.GetDocumentAsync($"/HarmedOrCasualty/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/HarmedOrCasualty/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string>
        {
            ["CasualtyId"] = id.ToString(),
            ["CasualtyName"] = "Atualizado",
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td")
                             .Select(td => td.TextContent.Trim());
        Assert.Contains("Atualizado", names);

        await DbUtilities.RemoveEntitiesAsync<HarmedOrCasualty>(dbContext);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var HarmedOrCasualty = new HarmedOrCasualty
        {
            CasualtyName = "Ferido"
        };

        _ = dbContext.Harmed_or_casualties.Add(HarmedOrCasualty);

        _ = await dbContext.SaveChangesAsync();

        var id = HarmedOrCasualty.CasualtyId.ToString()!;


        IDocument editDoc = await _client.GetDocumentAsync($"/HarmedOrCasualty/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/HarmedOrCasualty/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string>
        {
            ["CasualtyId"] = id.ToString(),
            ["HarmedOrCasualtyName"] = string.Empty
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        //Act
        IDocument doc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");

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

        await DbUtilities.RemoveEntitiesAsync<HarmedOrCasualty>(dbContext);
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var HarmedOrCasualty = new HarmedOrCasualty
        {
            CasualtyName = "Ferido"
        };

        _ = dbContext.Harmed_or_casualties.Add(HarmedOrCasualty);

        _ = await dbContext.SaveChangesAsync();

        var id = HarmedOrCasualty.CasualtyId.ToString()!;


        IDocument listDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");
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
        HttpResponseMessage postResponse = await _client.PostAsync($"/HarmedOrCasualty/Delete/{id}", content);

        // Assert 
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.DoesNotContain("Ferido", names);

        await DbUtilities.RemoveEntitiesAsync<HarmedOrCasualty>(dbContext);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Harmed_or_casualties.AddRange(
           new HarmedOrCasualty { CasualtyName = "Ferido" }
       );
        _ = await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");
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
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.Contains("Ferido", names);

        await DbUtilities.RemoveEntitiesAsync<HarmedOrCasualty>(dbContext);
    }

}

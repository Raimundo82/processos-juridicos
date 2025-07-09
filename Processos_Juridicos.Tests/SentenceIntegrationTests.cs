using System.Net;

using AngleSharp;
using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests;

public class SentenceIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_SentenceList_ReturnsSentenceList()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Sentences.AddRange(
            new Sentence { SentenceName = "Multa" },
            new Sentence { SentenceName = "Suspensão" }
        );
        await dbContext.SaveChangesAsync();

        //Act
        IDocument doc = await _client.GetDocumentAsync("/Sentence/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Multa", cellTexts);
        Assert.Contains("Suspensão", cellTexts);
        Assert.Equal(2, (await dbContext.Sentences.ToListAsync()).Count);
        await DbUtilities.RemoveEntitiesAsync<Sentence>(dbContext);
    }

    [Fact]
    public async Task List_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(await dbContext.Sentences.ToListAsync());

        // Act
        IDocument doc = await _client.GetDocumentAsync("/Sentence/List");

        // Assert
        IHtmlCollection<IElement> rows = doc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);

        IHtmlCollection<IElement> cells = doc.QuerySelectorAll("table tbody td");
        Assert.Empty(cells);

        Assert.Empty(await dbContext.Sentences.ToListAsync());
    }

    [Fact]
    public async Task Create_Post_Valid_RedirectsAndPersists()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var fields = new Dictionary<string, string>
        {
            ["SentenceName"] = "Multa"
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        HttpResponseMessage postResponse = await _client.PostAsync("/Sentence/Create", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        IDocument listDoc = await _client.GetDocumentAsync("/Sentence/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td").Select(td => td.TextContent.Trim());
        Assert.Contains("Multa", names);
        Assert.Single(await dbContext.Sentences.ToListAsync());

        await DbUtilities.RemoveEntitiesAsync<Sentence>(dbContext);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsEmptyList()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        IDocument getDoc = await _client.GetDocumentAsync("/Sentence/Create");
        IElement form = getDoc.QuerySelector("form[action='/Sentence/Create']")!;
        var action = form.GetAttribute("action")!;  // "/Sentence/Create"

        var fields = new Dictionary<string, string>
        {
            ["SentenceName"] = string.Empty
        };

        //Act
        HttpResponseMessage postResponse = await _client.PostAsync(
            action,
            new FormUrlEncodedContent(fields)
        );

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await _client.GetDocumentAsync("/Sentence/List");

        IHtmlCollection<IElement> rows = listDoc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);

        IHtmlCollection<IElement> cells = listDoc.QuerySelectorAll("table tbody td");
        Assert.Empty(cells);

        var html = await postResponse.Content.ReadAsStringAsync();
        IDocument errDoc = await BrowsingContext
                        .New(Configuration.Default)
                        .OpenAsync(req => req.Content(html));
        Assert.Contains(
            "O campo Nome da Sentença é obrigatório.",
            errDoc.DocumentElement.TextContent,
            StringComparison.OrdinalIgnoreCase
        );

        Assert.Empty(await dbContext.Sentences.ToListAsync());
        await DbUtilities.RemoveEntitiesAsync<HarmedOrCasualty>(dbContext);
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var Sentence = new Sentence
        {
            SentenceName = "Multa"
        };

        dbContext.Sentences.Add(Sentence);

        await dbContext.SaveChangesAsync();

        var id = Sentence.SentenceId;

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/Sentence/Edit/{id}");

        // Assert
        IElement? form = doc.QuerySelector("form[action^='/Sentence/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=SentenceId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=SentenceName]")!;
        Assert.Equal("Multa", nameInput.GetAttribute("value"));

        Assert.Single(await dbContext.Sentences.ToListAsync());

        await DbUtilities.RemoveEntitiesAsync<Sentence>(dbContext);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var Sentence = new Sentence
        {
            SentenceName = "Multa"
        };

        dbContext.Sentences.Add(Sentence);

        await dbContext.SaveChangesAsync();

        var id = Sentence.SentenceId.ToString()!;

        IDocument editDoc = await _client.GetDocumentAsync($"/Sentence/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/Sentence/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string>
        {
            ["SentenceId"] = id.ToString(),
            ["SentenceName"] = "Atualizado",
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await _client.GetDocumentAsync("/Sentence/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td").Select(td => td.TextContent.Trim());
        Assert.Contains("Atualizado", names);
        Assert.Single(await dbContext.Sentences.ToListAsync());
        await DbUtilities.RemoveEntitiesAsync<Sentence>(dbContext);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var Sentence = new Sentence
        {
            SentenceName = "Multa"
        };

        dbContext.Sentences.Add(Sentence);

        await dbContext.SaveChangesAsync();

        var id = Sentence.SentenceId.ToString()!;

        IDocument editDoc = await _client.GetDocumentAsync($"/Sentence/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/Sentence/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string>
        {
            ["SentenceId"] = id.ToString(),
            ["SentenceName"] = string.Empty
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        //Act
        IDocument doc = await _client.GetDocumentAsync("/Sentence/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Multa", cellTexts);
        Assert.Single(await dbContext.Sentences.ToListAsync());

        var html = await postResponse.Content.ReadAsStringAsync();
        IDocument errDoc = await BrowsingContext
                        .New(Configuration.Default)
                        .OpenAsync(req => req.Content(html));
        Assert.Contains(
            "O campo Nome da Sentença é obrigatório.",
            errDoc.DocumentElement.TextContent,
            StringComparison.OrdinalIgnoreCase
        );

        await DbUtilities.RemoveEntitiesAsync<Sentence>(dbContext);
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var id = dbContext.Sentences.Add(new Sentence { SentenceName = "Multa" }).Entity.SentenceId;
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/Sentence/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/Sentence/Delete']")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        // 2) POST the form with the correct Id & token
        var fields = new Dictionary<string, string?>
        {
            ["SentenceId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync($"/Sentence/Delete/{id}", content);

        // Assert 
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await _client.GetDocumentAsync("/Sentence/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.DoesNotContain("Multa", names);
        Assert.Empty(await dbContext.Sentences.ToListAsync());
        await DbUtilities.RemoveEntitiesAsync<Sentence>(dbContext);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Sentences.AddRange(
           new Sentence { SentenceName = "Multa" }
       );
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/Sentence/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/Sentence/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["SentenceId"] = "9999",
            ["__RequestVerificationToken"] = token
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await _client.GetDocumentAsync("/Sentence/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.Contains("Multa", names);
        Assert.Single(await dbContext.Sentences.ToListAsync());
        await DbUtilities.RemoveEntitiesAsync<Sentence>(dbContext);
    }

}

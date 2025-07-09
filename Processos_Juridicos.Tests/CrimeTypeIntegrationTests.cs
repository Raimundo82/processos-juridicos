using System.Net;

using AngleSharp;
using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests;

public class CrimeTypeIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_CrimeTypeList_ReturnsCrimeTypeList()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Crime_types.AddRange(
            new CrimeType { CrimeTypeName = "Corrupção" },
            new CrimeType { CrimeTypeName = "Fraude" }
        );
        await dbContext.SaveChangesAsync();

        //Act
        IDocument doc = await _client.GetDocumentAsync("/CrimeType/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Corrupção", cellTexts);
        Assert.Contains("Fraude", cellTexts);
        Assert.Equal(2, (await dbContext.Crime_types.ToListAsync()).Count);
        await DbUtilities.RemoveEntitiesAsync<CrimeType>(dbContext);
    }

    [Fact]
    public async Task List_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(await dbContext.Crime_types.ToListAsync());

        // Act
        IDocument doc = await _client.GetDocumentAsync("/CrimeType/List");

        // Assert
        IHtmlCollection<IElement> rows = doc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);

        IHtmlCollection<IElement> cells = doc.QuerySelectorAll("table tbody td");
        Assert.Empty(cells);

        Assert.Empty(await dbContext.Crime_types.ToListAsync());
    }

    [Fact]
    public async Task Create_Post_Valid_RedirectsAndPersists()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var fields = new Dictionary<string, string>
        {
            ["CrimeTypeName"] = "Assédio"
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        HttpResponseMessage postResponse = await _client.PostAsync("/CrimeType/Create", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        IDocument listDoc = await _client.GetDocumentAsync("/CrimeType/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td").Select(td => td.TextContent.Trim());
        Assert.Contains("Assédio", names);
        Assert.Single(await dbContext.Crime_types.ToListAsync());

        await DbUtilities.RemoveEntitiesAsync<CrimeType>(dbContext);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsEmptyList()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        IDocument getDoc = await _client.GetDocumentAsync("/CrimeType/Create");
        IElement form = getDoc.QuerySelector("form[action='/CrimeType/Create']")!;
        var action = form.GetAttribute("action")!;  // "/CrimeType/Create"

        var fields = new Dictionary<string, string>
        {
            ["CrimeTypeName"] = string.Empty
        };

        //Act
        HttpResponseMessage postResponse = await _client.PostAsync(action, new FormUrlEncodedContent(fields));

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await _client.GetDocumentAsync("/CrimeType/List");

        IHtmlCollection<IElement> rows = listDoc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);

        IHtmlCollection<IElement> cells = listDoc.QuerySelectorAll("table tbody td");
        Assert.Empty(cells);

        var html = await postResponse.Content.ReadAsStringAsync();
        IDocument errDoc = await BrowsingContext
                        .New(Configuration.Default)
                        .OpenAsync(req => req.Content(html));
        Assert.Contains(
            "O campo Nome do Tipo de Crime é obrigatório.",
            errDoc.DocumentElement.TextContent,
            StringComparison.OrdinalIgnoreCase
        );

        Assert.Empty(await dbContext.Crime_types.ToListAsync());
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var id = dbContext.Crime_types.Add(new CrimeType { CrimeTypeName = "Corrupção" }).Entity.CrimeTypeId;
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/CrimeType/Edit/{id}");

        // Assert
        IElement? form = doc.QuerySelector("form[action^='/CrimeType/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=CrimeTypeId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=CrimeTypeName]")!;
        Assert.Equal("Corrupção", nameInput.GetAttribute("value"));

        Assert.Single(await dbContext.Crime_types.ToListAsync());

        await DbUtilities.RemoveEntitiesAsync<CrimeType>(dbContext);
        Assert.Empty(await dbContext.Crime_types.ToListAsync());
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var id = dbContext.Crime_types.Add(new CrimeType { CrimeTypeName = "Corrupção" }).Entity.CrimeTypeId;
        await dbContext.SaveChangesAsync();


        IDocument editDoc = await _client.GetDocumentAsync($"/CrimeType/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/CrimeType/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["CrimeTypeId"] = id.ToString(),
            ["CrimeTypeName"] = "Atualizado",
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await _client.GetDocumentAsync("/CrimeType/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td").Select(td => td.TextContent.Trim());
        Assert.Contains("Atualizado", names);
        Assert.Single(await dbContext.Crime_types.ToListAsync());
        await DbUtilities.RemoveEntitiesAsync<CrimeType>(dbContext);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var id = dbContext.Crime_types.Add(new CrimeType { CrimeTypeName = "Corrupção" }).Entity.CrimeTypeId;

        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/CrimeType/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/CrimeType/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["CrimeTypeId"] = id.ToString(),
            ["CrimeTypeName"] = string.Empty
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        //Act
        IDocument doc = await _client.GetDocumentAsync("/CrimeType/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Corrupção", cellTexts);
        Assert.Single(await dbContext.Crime_types.ToListAsync());

        var html = await postResponse.Content.ReadAsStringAsync();
        IDocument errDoc = await BrowsingContext
                        .New(Configuration.Default)
                        .OpenAsync(req => req.Content(html));
        Assert.Contains(
            "O campo Nome do Tipo de Crime é obrigatório.",
            errDoc.DocumentElement.TextContent,
            StringComparison.OrdinalIgnoreCase
        );

        await DbUtilities.RemoveEntitiesAsync<CrimeType>(dbContext);
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var id = dbContext.Crime_types.Add(new CrimeType { CrimeTypeName = "Corrupção" }).Entity.CrimeTypeId;
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/CrimeType/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/CrimeType/Delete']")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        // 2) POST the form with the correct Id & token
        var fields = new Dictionary<string, string?>
        {
            ["CrimeTypeId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync("/CrimeType/Delete/1", content);

        // Assert 
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await _client.GetDocumentAsync("/CrimeType/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.DoesNotContain("Corrupção", names);
        Assert.Empty(await dbContext.Crime_types.ToListAsync());
        await DbUtilities.RemoveEntitiesAsync<CrimeType>(dbContext);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Crime_types.AddRange(
           new CrimeType { CrimeTypeName = "Corrupção" }
       );
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/CrimeType/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/CrimeType/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["CrimeTypeId"] = "9999",
            ["__RequestVerificationToken"] = token
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await _client.GetDocumentAsync("/CrimeType/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.Contains("Corrupção", names);
        Assert.Single(await dbContext.Crime_types.ToListAsync());
        await DbUtilities.RemoveEntitiesAsync<CrimeType>(dbContext);
    }

}

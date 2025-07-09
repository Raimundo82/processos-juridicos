using System.Net;

using AngleSharp;
using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests;

public class AccidentTypeIntegrationTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_AccidentTypeList_ReturnsAccidentTypeList()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Accident_types.AddRange(
            new AccidentType { AccidentTypeName = "Viação" },
            new AccidentType { AccidentTypeName = "Serviço Guarnição" }
        );
        await dbContext.SaveChangesAsync();

        //Act
        IDocument doc = await _client.GetDocumentAsync("/AccidentType/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Viação", cellTexts);
        Assert.Contains("Serviço Guarnição", cellTexts);
        Assert.Equal(2, (await dbContext.Accident_types.ToListAsync()).Count);
        await DbUtilities.RemoveEntitiesAsync<AccidentType>(dbContext);
    }

    [Fact]
    public async Task List_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(await dbContext.Accident_types.ToListAsync());

        // Act
        IDocument doc = await _client.GetDocumentAsync("/AccidentType/List");

        // Assert
        IHtmlCollection<IElement> rows = doc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);

        IHtmlCollection<IElement> cells = doc.QuerySelectorAll("table tbody td");
        Assert.Empty(cells);

        Assert.Empty(await dbContext.Accident_types.ToListAsync());
    }

    [Fact]
    public async Task Create_Post_Valid_RedirectsAndPersists()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var fields = new Dictionary<string, string>
        {
            ["AccidentTypeName"] = "Viação"
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        HttpResponseMessage postResponse = await _client.PostAsync("/AccidentType/Create", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        IDocument listDoc = await _client.GetDocumentAsync("/AccidentType/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td").Select(td => td.TextContent.Trim());
        Assert.Contains("Viação", names);
        Assert.Single(await dbContext.Accident_types.ToListAsync());

        await DbUtilities.RemoveEntitiesAsync<AccidentType>(dbContext);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsEmptyList()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        IDocument getDoc = await _client.GetDocumentAsync("/AccidentType/Create");
        IElement form = getDoc.QuerySelector("form[action='/AccidentType/Create']")!;
        var action = form.GetAttribute("action")!;  // "/AccidentType/Create"

        var fields = new Dictionary<string, string>
        {
            ["AccidentTypeName"] = string.Empty
        };

        //Act
        HttpResponseMessage postResponse = await _client.PostAsync(
            action,
            new FormUrlEncodedContent(fields)
        );

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await _client.GetDocumentAsync("/AccidentType/List");

        IHtmlCollection<IElement> rows = listDoc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);

        IHtmlCollection<IElement> cells = listDoc.QuerySelectorAll("table tbody td");
        Assert.Empty(cells);

        var html = await postResponse.Content.ReadAsStringAsync();
        IDocument errDoc = await BrowsingContext
                        .New(Configuration.Default)
                        .OpenAsync(req => req.Content(html));
        Assert.Contains(
            "O campo Nome do Tipo de Acidente é obrigatório.",
            errDoc.DocumentElement.TextContent,
            StringComparison.OrdinalIgnoreCase
        );
        Assert.Empty(await dbContext.Accident_types.ToListAsync());

    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var AccidentType = new AccidentType
        {
            AccidentTypeName = "Viação"
        };

        dbContext.Accident_types.Add(AccidentType);

        await dbContext.SaveChangesAsync();

        var id = AccidentType.AccidentTypeId;

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/AccidentType/Edit/{id}");

        // Assert
        IElement? form = doc.QuerySelector("form[action^='/AccidentType/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=AccidentTypeId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=AccidentTypeName]")!;
        Assert.Equal("Viação", nameInput.GetAttribute("value"));

        Assert.Single(await dbContext.Accident_types.ToListAsync());

        await DbUtilities.RemoveEntitiesAsync<AccidentType>(dbContext);
        Assert.Empty(await dbContext.Accident_types.ToListAsync());
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var AccidentType = new AccidentType
        {
            AccidentTypeName = "Viação"
        };

        dbContext.Accident_types.Add(AccidentType);

        await dbContext.SaveChangesAsync();

        var id = AccidentType.AccidentTypeId.ToString()!;

        IDocument editDoc = await _client.GetDocumentAsync($"/AccidentType/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/AccidentType/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string>
        {
            ["AccidentTypeId"] = id.ToString(),
            ["AccidentTypeName"] = "Atualizado",
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await _client.GetDocumentAsync("/AccidentType/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td").Select(td => td.TextContent.Trim());
        Assert.Contains("Atualizado", names);
        Assert.Single(await dbContext.Accident_types.ToListAsync());

        await DbUtilities.RemoveEntitiesAsync<AccidentType>(dbContext);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var id = dbContext.Accident_types.Add(new AccidentType { AccidentTypeName = "Viação" }).Entity.AccidentTypeId;

        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/AccidentType/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/AccidentType/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["AccidentTypeId"] = id.ToString(),
            ["AccidentTypeName"] = string.Empty
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        //Act
        IDocument doc = await _client.GetDocumentAsync("/AccidentType/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Viação", cellTexts);
        Assert.Single(await dbContext.Accident_types.ToListAsync());

        var html = await postResponse.Content.ReadAsStringAsync();
        IDocument errDoc = await BrowsingContext
                        .New(Configuration.Default)
                        .OpenAsync(req => req.Content(html));
        Assert.Contains(
            "O campo Nome do Tipo de Acidente é obrigatório.",
            errDoc.DocumentElement.TextContent,
            StringComparison.OrdinalIgnoreCase
        );

        await DbUtilities.RemoveEntitiesAsync<AccidentType>(dbContext);
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var id = dbContext.Accident_types.Add(new AccidentType { AccidentTypeName = "Viação" }).Entity.AccidentTypeId;

        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/AccidentType/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/AccidentType/Delete']")!;

        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        // 2) POST the form with the correct Id & token
        var fields = new Dictionary<string, string?>
        {
            ["AccidentTypeId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync($"/AccidentType/Delete/{id}", content);

        // Assert 
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await _client.GetDocumentAsync("/AccidentType/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.DoesNotContain("Viação", names);
        Assert.Empty(await dbContext.Accident_types.ToListAsync());
        await DbUtilities.RemoveEntitiesAsync<AccidentType>(dbContext);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Accident_types.AddRange(
           new AccidentType { AccidentTypeName = "Viação" }
       );
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/AccidentType/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/AccidentType/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["AccidentTypeId"] = "9999",
            ["__RequestVerificationToken"] = token
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await _client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await _client.GetDocumentAsync("/AccidentType/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.Contains("Viação", names);
        Assert.Single(await dbContext.Accident_types.ToListAsync());
        await DbUtilities.RemoveEntitiesAsync<AccidentType>(dbContext);
    }

}

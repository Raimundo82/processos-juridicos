using System.Net;

using AngleSharp;
using AngleSharp.Dom;

using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests;

public class CrimeTypeIntegrationTests
{
    [Fact]
    public async Task Get_CrimeTypeList_ReturnsCrimeTypeList()
    {

        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Crime_types.AddRange(
            new CrimeType { CrimeTypeName = "Corrupção" },
            new CrimeType { CrimeTypeName = "Fraude" }
        );
        _ = await db.SaveChangesAsync();

        using HttpClient client = factory.CreateClient();

        //Act
        IDocument doc = await client.GetDocumentAsync("/CrimeType/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Corrupção", cellTexts);
        Assert.Contains("Fraude", cellTexts);
    }

    [Fact]
    public async Task List_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using HttpClient client = factory.CreateClient();

        // Act
        IDocument doc = await client.GetDocumentAsync("/CrimeType/List");

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
            ["CrimeTypeName"] = "Assédio"
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        HttpResponseMessage postResponse = await client.PostAsync("/CrimeType/Create", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        IDocument listDoc = await client.GetDocumentAsync("/CrimeType/List");
        IEnumerable<string> names = listDoc.QuerySelectorAll("table tbody td")
                             .Select(td => td.TextContent.Trim());
        Assert.Contains("Assédio", names);
    }
    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsEmptyList()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using HttpClient client = factory.CreateClient();

        IDocument getDoc = await client.GetDocumentAsync("/CrimeType/Create");
        IElement form = getDoc.QuerySelector("form[action='/CrimeType/Create']")!;
        var action = form.GetAttribute("action")!;  // "/CrimeType/Create"

        var fields = new Dictionary<string, string>
        {
            ["CrimeTypeName"] = string.Empty
        };

        //Act
        HttpResponseMessage postResponse = await client.PostAsync(
            action,
            new FormUrlEncodedContent(fields)
        );

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await client.GetDocumentAsync("/CrimeType/List");

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
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var crimeType = new CrimeType
        {
            CrimeTypeName = "Corrupção"
        };

        _ = db.Crime_types.Add(crimeType);

        _ = await db.SaveChangesAsync();

        var id = crimeType.CrimeTypeId;

        using HttpClient client = factory.CreateClient();

        // Act
        IDocument doc = await client.GetDocumentAsync($"/CrimeType/Edit/{id}");

        // Assert
        IElement? form = doc.QuerySelector("form[action^='/CrimeType/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=CrimeTypeId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=CrimeTypeName]")!;
        Assert.Equal("Corrupção", nameInput.GetAttribute("value"));
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var crimeType = new CrimeType
        {
            CrimeTypeName = "Corrupção"
        };

        _ = db.Crime_types.Add(crimeType);

        _ = await db.SaveChangesAsync();

        var id = crimeType.CrimeTypeId.ToString()!;

        using HttpClient client = factory.CreateClient();
        IDocument editDoc = await client.GetDocumentAsync($"/CrimeType/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/CrimeType/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string>
        {
            ["CrimeTypeId"] = id.ToString(),
            ["CrimeTypeName"] = "Atualizado",
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await client.PostAsync(action, content);

        //Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument listDoc = await client.GetDocumentAsync("/CrimeType/List");
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

        var crimeType = new CrimeType
        {
            CrimeTypeName = "Corrupção"
        };

        _ = db.Crime_types.Add(crimeType);

        _ = await db.SaveChangesAsync();

        var id = crimeType.CrimeTypeId.ToString()!;

        using HttpClient client = factory.CreateClient();

        IDocument editDoc = await client.GetDocumentAsync($"/CrimeType/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/CrimeType/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string>
        {
            ["CrimeTypeId"] = id.ToString(),
            ["CrimeTypeName"] = string.Empty
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        //Act
        IDocument doc = await client.GetDocumentAsync("/CrimeType/List");

        // Assert
        var cellTexts = doc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim())
            .ToList();

        Assert.Contains("Corrupção", cellTexts);

        var html = await postResponse.Content.ReadAsStringAsync();
        IDocument errDoc = await BrowsingContext
                        .New(Configuration.Default)
                        .OpenAsync(req => req.Content(html));
        Assert.Contains(
            "O campo Nome do Tipo de Crime é obrigatório.",
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

        var crimeType = new CrimeType
        {
            CrimeTypeName = "Corrupção"
        };

        _ = db.Crime_types.Add(crimeType);

        _ = await db.SaveChangesAsync();

        var id = crimeType.CrimeTypeId.ToString()!;

        using HttpClient client = factory.CreateClient();

        IDocument listDoc = await client.GetDocumentAsync("/CrimeType/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/CrimeType/Delete']")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        // 2) POST the form with the correct Id & token
        var fields = new Dictionary<string, string>
        {
            ["CrimeTypeId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        var content = new FormUrlEncodedContent(fields);
        HttpResponseMessage postResponse = await client.PostAsync("/CrimeType/Delete/1", content);

        // Assert 
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await client.GetDocumentAsync("/CrimeType/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.DoesNotContain("Corrupção", names);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        using HttpClient client = factory.CreateClient();

        db.Crime_types.AddRange(
           new CrimeType { CrimeTypeName = "Corrupção" }
       );
        _ = await db.SaveChangesAsync();

        IDocument listDoc = await client.GetDocumentAsync("/CrimeType/List");
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
        HttpResponseMessage postResponse = await client.PostAsync(action, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        IDocument afterDoc = await client.GetDocumentAsync("/CrimeType/List");
        IEnumerable<string> names = afterDoc
            .QuerySelectorAll("table tbody td")
            .Select(td => td.TextContent.Trim());
        Assert.Contains("Corrupção", names);
    }

}

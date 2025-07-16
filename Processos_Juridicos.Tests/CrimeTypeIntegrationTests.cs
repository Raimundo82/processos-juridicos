using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests;

public class CrimeTypeIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    private static CrimeType CreateCrimeType(string name)
    {
        return new CrimeType { CrimeTypeName = name };
    }

    [Theory]
    [InlineData()]
    [InlineData("Corrupção", "Fraude")]
    public async Task List_ReturnsExpectedItems(params string[] namesInput)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Crime_types.AddRange(namesInput.Select(CreateCrimeType));
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync("/CrimeType/List");

        // Assert
        Assert.Equal(namesInput.Length, await dbContext.Crime_types.CountAsync());

        var rows = doc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (CrimeType CrimeType in dbContext.Crime_types)
        {
            Assert.Contains(CrimeType.CrimeTypeName, namesInput);

            IElement? row = doc.QuerySelector($"table>tbody>tr[data-id='{CrimeType.CrimeTypeId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector($"td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(CrimeType.CrimeTypeName, cell.TextContent.Trim());
        }
    }

    [Theory]
    [InlineData()]
    [InlineData("Corrupção")]
    [InlineData("Corrupção", "Fraude")]
    public async Task Create_Post_CreatesExpectedItems(params string[] namesInput)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //Act
        foreach (var name in namesInput)
        {
            var formData = new Dictionary<string, string>
            {
                ["CrimeTypeName"] = name
            };

            await _client.PostAsync("/CrimeType/Create", new FormUrlEncodedContent(formData));
        }

        // Assert
        DbSet<CrimeType> dbItems = dbContext.Crime_types;

        Assert.Equal(namesInput.Length, dbItems.Count());

        IDocument listDoc = await _client.GetDocumentAsync("/CrimeType/List");
        var rows = listDoc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (CrimeType at in dbItems)
        {
            Assert.Contains(at.CrimeTypeName, namesInput);

            IElement? row = listDoc
                .QuerySelector($"table > tbody > tr[data-id='{at.CrimeTypeId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(at.CrimeTypeName, cell.TextContent.Trim());
        }
    }


    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        CrimeType CrimeType = CreateCrimeType("Corrupção");

        dbContext.Crime_types.Add(CrimeType);

        await dbContext.SaveChangesAsync();

        var id = CrimeType.CrimeTypeId;

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/CrimeType/Edit/{id}");

        // Assert
        Assert.Single(dbContext.Crime_types);
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
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        CrimeType CrimeType = CreateCrimeType("Corrupção");

        dbContext.Crime_types.Add(CrimeType);

        await dbContext.SaveChangesAsync();

        var id = CrimeType.CrimeTypeId;

        IDocument editDoc = await _client.GetDocumentAsync($"/CrimeType/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/CrimeType/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["CrimeTypeId"] = id.ToString(),
            ["CrimeTypeName"] = "Atualizado",
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        await _client.PostAsync(action, content);
        IDocument listDoc = await _client.GetDocumentAsync("/CrimeType/List");

        //Assert
        Assert.Single(dbContext.Crime_types);
        IElement? cell = listDoc.QuerySelector("table tbody td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal("Atualizado", cell.TextContent.Trim());
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var crimeTypeName = "Corrupção";
        CrimeType crimeType = CreateCrimeType(crimeTypeName);
        dbContext.Crime_types.Add(crimeType);
        var id = crimeType.CrimeTypeId;
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
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument doc = await _client.GetDocumentAsync("/CrimeType/List");

        // Assert
        Assert.Single(dbContext.Crime_types);

        IElement? row = doc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);
        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal(cell.TextContent.Trim(), crimeTypeName);
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        CrimeType CrimeType = CreateCrimeType("Corrupção");
        dbContext.Crime_types.Add(CrimeType);
        await dbContext.SaveChangesAsync();
        var id = CrimeType.CrimeTypeId;

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
        await _client.PostAsync($"/CrimeType/Delete/{id}", new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/CrimeType/List");

        // Assert 
        Assert.Empty(dbContext.Crime_types);
        IHtmlCollection<IElement> rows = afterDoc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        CrimeType CrimeType = CreateCrimeType("Corrupção");
        dbContext.Crime_types.Add(CrimeType);
        await dbContext.SaveChangesAsync();
        var id = CrimeType.CrimeTypeId;

        IDocument listDoc = await _client.GetDocumentAsync("/CrimeType/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/CrimeType/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["CrimeTypeId"] = "-1",
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/CrimeType/List");

        // Assert
        Assert.Single(dbContext.Crime_types);
        IElement? row = afterDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);
        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal("Corrupção", cell.TextContent.Trim());
    }


    public async Task InitializeAsync()
    {
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.RemoveRange(dbContext.Crime_types);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

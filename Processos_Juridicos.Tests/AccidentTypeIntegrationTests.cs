using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests;

public class AccidentTypeIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    private static AccidentType CreateAccidentType(string name)
    {
        return new AccidentType { AccidentTypeName = name };
    }

    [Theory]
    [InlineData()]
    [InlineData("Viação", "Serviço")]
    public async Task List_ReturnsExpectedItems(params string[] namesInput)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.AccidentTypes.AddRange(namesInput.Select(CreateAccidentType));
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync("/AccidentType/List");

        // Assert
        Assert.Equal(namesInput.Length, await dbContext.AccidentTypes.CountAsync());

        var rows = doc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (AccidentType accidentType in dbContext.AccidentTypes)
        {
            Assert.Contains(accidentType.AccidentTypeName, namesInput);

            IElement? row = doc.QuerySelector($"table>tbody>tr[data-id='{accidentType.AccidentTypeId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector($"td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(accidentType.AccidentTypeName, cell.TextContent.Trim());
        }
    }

    [Theory]
    [InlineData()]
    [InlineData("Viação")]
    [InlineData("Viação", "Serviço")]
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
                ["AccidentTypeName"] = name
            };

            await _client.PostAsync("/AccidentType/Create", new FormUrlEncodedContent(formData));
        }

        // Assert
        DbSet<AccidentType> dbItems = dbContext.AccidentTypes;

        Assert.Equal(namesInput.Length, dbItems.Count());

        IDocument listDoc = await _client.GetDocumentAsync("/AccidentType/List");
        var rows = listDoc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (AccidentType at in dbItems)
        {
            Assert.Contains(at.AccidentTypeName, namesInput);

            IElement? row = listDoc
                .QuerySelector($"table > tbody > tr[data-id='{at.AccidentTypeId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(at.AccidentTypeName, cell.TextContent.Trim());
        }
    }


    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        AccidentType accidentType = CreateAccidentType("Viação");

        dbContext.AccidentTypes.Add(accidentType);

        await dbContext.SaveChangesAsync();

        var id = accidentType.AccidentTypeId;

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/AccidentType/Edit/{id}");

        // Assert
        Assert.Single(dbContext.AccidentTypes);
        IElement? form = doc.QuerySelector("form[action^='/AccidentType/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=AccidentTypeId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=AccidentTypeName]")!;
        Assert.Equal("Viação", nameInput.GetAttribute("value"));
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        AccidentType AccidentType = CreateAccidentType("Viação");

        dbContext.AccidentTypes.Add(AccidentType);

        await dbContext.SaveChangesAsync();

        var id = AccidentType.AccidentTypeId;

        IDocument editDoc = await _client.GetDocumentAsync($"/AccidentType/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/AccidentType/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["AccidentTypeId"] = id.ToString(),
            ["AccidentTypeName"] = "Atualizado",
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        await _client.PostAsync(action, content);
        IDocument listDoc = await _client.GetDocumentAsync("/AccidentType/List");

        //Assert
        Assert.Single(dbContext.AccidentTypes);
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

        var accidentTypeName = "serviço";
        AccidentType accidentType = CreateAccidentType(accidentTypeName);
        dbContext.AccidentTypes.Add(accidentType);
        var id = accidentType.AccidentTypeId;
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
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument doc = await _client.GetDocumentAsync("/AccidentType/List");

        // Assert
        Assert.Single(dbContext.AccidentTypes);

        IElement? row = doc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);
        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal(cell.TextContent.Trim(), accidentTypeName);
    }


    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        AccidentType AccidentType = CreateAccidentType("Viação");
        dbContext.AccidentTypes.Add(AccidentType);
        await dbContext.SaveChangesAsync();
        var id = AccidentType.AccidentTypeId;

        IDocument listDoc = await _client.GetDocumentAsync("/AccidentType/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/AccidentType/Delete']")!;

        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string?>
        {
            ["AccidentTypeId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/AccidentType/Delete/{id}", new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/AccidentType/List");

        // Assert 
        Assert.Empty(dbContext.AccidentTypes);
        IHtmlCollection<IElement> rows = afterDoc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        AccidentType AccidentType = CreateAccidentType("Viação");
        dbContext.AccidentTypes.Add(AccidentType);
        await dbContext.SaveChangesAsync();
        var id = AccidentType.AccidentTypeId;

        IDocument listDoc = await _client.GetDocumentAsync("/AccidentType/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/AccidentType/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["AccidentTypeId"] = "-1",
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/AccidentType/List");

        // Assert
        Assert.Single(dbContext.AccidentTypes);
        IElement? row = afterDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);
        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal("Viação", cell.TextContent.Trim());
    }

    public async Task InitializeAsync()
    {
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.RemoveRange(dbContext.AccidentTypes);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

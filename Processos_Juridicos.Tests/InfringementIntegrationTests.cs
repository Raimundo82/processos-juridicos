using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Tests.TestHelpers;

namespace Processos_Juridicos.Tests;

public class InfringementIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    private static Infringement CreateInfringement(string name)
    {
        return new Infringement { InfringementName = name };
    }

    [Theory]
    [InlineData()]
    [InlineData("11º Artigo", "12º Artigo")]
    public async Task List_ReturnsExpectedItems(params string[] namesInput)
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Infringements.AddRange(namesInput.Select(CreateInfringement));
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync("/Infringement/List");

        // Assert
        Assert.Equal(namesInput.Length, await dbContext.Infringements.CountAsync());

        var rows = doc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (Infringement infringement in dbContext.Infringements)
        {
            Assert.Contains(infringement.InfringementName, namesInput);

            IElement? row = doc.QuerySelector($"table>tbody>tr[data-id='{infringement.InfringementId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector($"td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(infringement.InfringementName, cell.TextContent.Trim());
        }
    }

    [Theory]
    [InlineData()]
    [InlineData("11º Artigo")]
    [InlineData("11º Artigo", "12º Artigo")]
    public async Task Create_Post_CreatesExpectedItems(params string[] namesInput)
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //Act
        foreach (var name in namesInput)
        {
            var formData = new Dictionary<string, string>
            {
                ["InfringementName"] = name
            };

            await _client.PostAsync("/Infringement/Create", new FormUrlEncodedContent(formData));
        }

        // Assert
        DbSet<Infringement> dbItems = dbContext.Infringements;

        Assert.Equal(namesInput.Length, dbItems.Count());

        IDocument listDoc = await _client.GetDocumentAsync("/Infringement/List");
        var rows = listDoc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (Infringement at in dbItems)
        {
            Assert.Contains(at.InfringementName, namesInput);

            IElement? row = listDoc
                .QuerySelector($"table > tbody > tr[data-id='{at.InfringementId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(at.InfringementName, cell.TextContent.Trim());
        }
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Infringement infringement = CreateInfringement("11º Artigo");

        dbContext.Infringements.Add(infringement);

        await dbContext.SaveChangesAsync();

        var id = infringement.InfringementId;

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/Infringement/Edit/{id}");

        // Assert
        Assert.Single(dbContext.Infringements);
        IElement? form = doc.QuerySelector("form[action^='/Infringement/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=InfringementId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=InfringementName]")!;
        Assert.Equal("11º Artigo", nameInput.GetAttribute("value"));
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Infringement Infringement = CreateInfringement("11º Artigo");

        dbContext.Infringements.Add(Infringement);

        await dbContext.SaveChangesAsync();

        var id = Infringement.InfringementId;

        IDocument editDoc = await _client.GetDocumentAsync($"/Infringement/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/Infringement/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["InfringementId"] = id.ToString(),
            ["InfringementName"] = "Atualizado",
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        await _client.PostAsync(action, content);
        IDocument listDoc = await _client.GetDocumentAsync("/Infringement/List");

        //Assert
        Assert.Single(dbContext.Infringements);
        IElement? cell = listDoc.QuerySelector("table tbody td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal("Atualizado", cell.TextContent.Trim());
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var infringementName = "11º Artigo";
        Infringement infringement = CreateInfringement(infringementName);
        dbContext.Infringements.Add(infringement);
        var id = infringement.InfringementId;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/Infringement/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/Infringement/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["InfringementId"] = id.ToString(),
            ["InfringementName"] = string.Empty
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument doc = await _client.GetDocumentAsync("/Infringement/List");

        // Assert
        Assert.Single(dbContext.Infringements);

        IElement? row = doc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);
        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal(cell.TextContent.Trim(), infringementName);
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Infringement Infringement = CreateInfringement("11º Artigo");
        dbContext.Infringements.Add(Infringement);
        await dbContext.SaveChangesAsync();
        var id = Infringement.InfringementId;

        IDocument listDoc = await _client.GetDocumentAsync("/Infringement/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/Infringement/Delete']")!;

        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string?>
        {
            ["InfringementId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/Infringement/Delete/{id}", new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/Infringement/List");

        // Assert 
        Assert.Empty(dbContext.Infringements);
        IHtmlCollection<IElement> rows = afterDoc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Infringement Infringement = CreateInfringement("11º Artigo");
        dbContext.Infringements.Add(Infringement);
        await dbContext.SaveChangesAsync();
        var id = Infringement.InfringementId;

        IDocument listDoc = await _client.GetDocumentAsync("/Infringement/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/Infringement/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["InfringementId"] = "-1",
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/Infringement/List");

        // Assert
        Assert.Single(dbContext.Infringements);
        IElement? row = afterDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);
        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal("11º Artigo", cell.TextContent.Trim());
    }


    public async Task InitializeAsync()
    {
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.RemoveRange(dbContext.Infringements);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

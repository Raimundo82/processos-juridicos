using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests;

public class MilitarySecurityIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    private static MilitarySecurity CreateMilitarySecurity(string name)
    {
        return new MilitarySecurity { MilitarySecurityName = name };
    }

    [Theory]
    [InlineData()]
    [InlineData("Incidentes entre militares", "Outros")]
    public async Task List_ReturnsExpectedItems(params string[] namesInput)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Military_securities.AddRange(namesInput.Select(CreateMilitarySecurity));
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync("/MilitarySecurity/List");

        // Assert
        Assert.Equal(namesInput.Length, await dbContext.Military_securities.CountAsync());

        var rows = doc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (MilitarySecurity militarySecurity in dbContext.Military_securities)
        {
            Assert.Contains(militarySecurity.MilitarySecurityName, namesInput);

            IElement? row = doc.QuerySelector($"table>tbody>tr[data-id='{militarySecurity.MilitarySecurityId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector($"td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(militarySecurity.MilitarySecurityName, cell.TextContent.Trim());
        }
    }

    [Theory]
    [InlineData()]
    [InlineData("Incidentes entre militares")]
    [InlineData("Incidentes entre militares", "Outros")]
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
                ["MilitarySecurityName"] = name
            };

            await _client.PostAsync("/MilitarySecurity/Create", new FormUrlEncodedContent(formData));
        }

        // Assert
        DbSet<MilitarySecurity> dbItems = dbContext.Military_securities;

        Assert.Equal(namesInput.Length, dbItems.Count());

        IDocument listDoc = await _client.GetDocumentAsync("/MilitarySecurity/List");
        var rows = listDoc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (MilitarySecurity at in dbItems)
        {
            Assert.Contains(at.MilitarySecurityName, namesInput);

            IElement? row = listDoc
                .QuerySelector($"table > tbody > tr[data-id='{at.MilitarySecurityId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(at.MilitarySecurityName, cell.TextContent.Trim());
        }
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        MilitarySecurity militarySecurity = CreateMilitarySecurity("Incidentes entre militares");

        dbContext.Military_securities.Add(militarySecurity);

        await dbContext.SaveChangesAsync();

        var id = militarySecurity.MilitarySecurityId;

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/MilitarySecurity/Edit/{id}");

        // Assert
        Assert.Single(dbContext.Military_securities);
        IElement? form = doc.QuerySelector("form[action^='/MilitarySecurity/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=MilitarySecurityId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=MilitarySecurityName]")!;
        Assert.Equal("Incidentes entre militares", nameInput.GetAttribute("value"));
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        MilitarySecurity MilitarySecurity = CreateMilitarySecurity("Incidentes entre militares");

        dbContext.Military_securities.Add(MilitarySecurity);

        await dbContext.SaveChangesAsync();

        var id = MilitarySecurity.MilitarySecurityId;

        IDocument editDoc = await _client.GetDocumentAsync($"/MilitarySecurity/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/MilitarySecurity/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["MilitarySecurityId"] = id.ToString(),
            ["MilitarySecurityName"] = "Atualizado",
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        await _client.PostAsync(action, content);
        IDocument listDoc = await _client.GetDocumentAsync("/MilitarySecurity/List");

        //Assert
        Assert.Single(dbContext.Military_securities);
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

        var sentenceName = "prisão";
        MilitarySecurity sentence = CreateMilitarySecurity(sentenceName);
        dbContext.Military_securities.Add(sentence);
        var id = sentence.MilitarySecurityId;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/MilitarySecurity/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/MilitarySecurity/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["MilitarySecurityId"] = id.ToString(),
            ["MilitarySecurityName"] = string.Empty
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument doc = await _client.GetDocumentAsync("/MilitarySecurity/List");

        // Assert
        Assert.Single(dbContext.Military_securities);

        IElement? row = doc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);
        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal(cell.TextContent.Trim(), sentenceName);
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        MilitarySecurity MilitarySecurity = CreateMilitarySecurity("Incidentes entre militares");
        dbContext.Military_securities.Add(MilitarySecurity);
        await dbContext.SaveChangesAsync();
        var id = MilitarySecurity.MilitarySecurityId;

        IDocument listDoc = await _client.GetDocumentAsync("/MilitarySecurity/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/MilitarySecurity/Delete']")!;

        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        // 2) POST the form with the correct Id & token
        var fields = new Dictionary<string, string?>
        {
            ["MilitarySecurityId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/MilitarySecurity/Delete/{id}", new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/MilitarySecurity/List");

        // Assert 
        Assert.Empty(dbContext.Military_securities);
        IHtmlCollection<IElement> rows = afterDoc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        MilitarySecurity MilitarySecurity = CreateMilitarySecurity("Incidentes entre militares");
        dbContext.Military_securities.Add(MilitarySecurity);
        await dbContext.SaveChangesAsync();
        var id = MilitarySecurity.MilitarySecurityId;

        IDocument listDoc = await _client.GetDocumentAsync("/MilitarySecurity/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/MilitarySecurity/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["MilitarySecurityId"] = "-1",
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/MilitarySecurity/List");

        // Assert
        Assert.Single(dbContext.Military_securities);
        IElement? row = afterDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);
        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal("Incidentes entre militares", cell.TextContent.Trim());
    }


    public async Task InitializeAsync()
    {
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.RemoveRange(dbContext.Military_securities);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

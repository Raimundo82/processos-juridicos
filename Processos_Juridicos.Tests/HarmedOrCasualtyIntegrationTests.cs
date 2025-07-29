using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests;

public class HarmedOrCasualtyIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    private static HarmedOrCasualty CreateHarmedOrCasualty(string name)
    {
        return new HarmedOrCasualty { CasualtyName = name };
    }

    [Theory]
    [InlineData()]
    [InlineData("Ferido", "Outros")]
    public async Task List_ReturnsExpectedItems(params string[] namesInput)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.HarmedOrCasualties.AddRange(namesInput.Select(CreateHarmedOrCasualty));
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");

        // Assert
        Assert.Equal(namesInput.Length, await dbContext.HarmedOrCasualties.CountAsync());

        var rows = doc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (HarmedOrCasualty HarmedOrCasualty in dbContext.HarmedOrCasualties)
        {
            Assert.Contains(HarmedOrCasualty.CasualtyName, namesInput);

            IElement? row = doc.QuerySelector($"table>tbody>tr[data-id='{HarmedOrCasualty.CasualtyId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector($"td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(HarmedOrCasualty.CasualtyName, cell.TextContent.Trim());
        }
    }

    [Theory]
    [InlineData()]
    [InlineData("Ferido")]
    [InlineData("Ferido", "Outros")]
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
                ["CasualtyName"] = name
            };

            await _client.PostAsync("/HarmedOrCasualty/Create", new FormUrlEncodedContent(formData));
        }

        // Assert
        DbSet<HarmedOrCasualty> dbItems = dbContext.HarmedOrCasualties;

        Assert.Equal(namesInput.Length, dbItems.Count());

        IDocument listDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");
        var rows = listDoc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (HarmedOrCasualty at in dbItems)
        {
            Assert.Contains(at.CasualtyName, namesInput);

            IElement? row = listDoc
                .QuerySelector($"table > tbody > tr[data-id='{at.CasualtyId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(at.CasualtyName, cell.TextContent.Trim());
        }
    }


    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        HarmedOrCasualty HarmedOrCasualty = CreateHarmedOrCasualty("Ferido");

        dbContext.HarmedOrCasualties.Add(HarmedOrCasualty);

        await dbContext.SaveChangesAsync();

        var id = HarmedOrCasualty.CasualtyId;

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/HarmedOrCasualty/Edit/{id}");

        // Assert
        Assert.Single(dbContext.HarmedOrCasualties);
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
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        HarmedOrCasualty HarmedOrCasualty = CreateHarmedOrCasualty("Ferido");

        dbContext.HarmedOrCasualties.Add(HarmedOrCasualty);

        await dbContext.SaveChangesAsync();

        var id = HarmedOrCasualty.CasualtyId;

        IDocument editDoc = await _client.GetDocumentAsync($"/HarmedOrCasualty/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/HarmedOrCasualty/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["CasualtyId"] = id.ToString(),
            ["CasualtyName"] = "Atualizado",
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        await _client.PostAsync(action, content);
        IDocument listDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");

        //Assert
        Assert.Single(dbContext.HarmedOrCasualties);
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

        var casualtyName = "Ferido";
        HarmedOrCasualty harmedOrCasualty = CreateHarmedOrCasualty(casualtyName);
        dbContext.HarmedOrCasualties.Add(harmedOrCasualty);
        var id = harmedOrCasualty.CasualtyId;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/HarmedOrCasualty/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/HarmedOrCasualty/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["CasualtyId"] = id.ToString(),
            ["CasualtyName"] = string.Empty
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument doc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");

        // Assert
        Assert.Single(dbContext.HarmedOrCasualties);

        IElement? row = doc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);
        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal(cell.TextContent.Trim(), casualtyName);
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        HarmedOrCasualty HarmedOrCasualty = CreateHarmedOrCasualty("Ferido");
        dbContext.HarmedOrCasualties.Add(HarmedOrCasualty);
        await dbContext.SaveChangesAsync();
        var id = HarmedOrCasualty.CasualtyId;

        IDocument listDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/HarmedOrCasualty/Delete']")!;

        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        // 2) POST the form with the correct Id & token
        var fields = new Dictionary<string, string?>
        {
            ["CasualtyId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/HarmedOrCasualty/Delete/{id}", new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");

        // Assert 
        Assert.Empty(dbContext.HarmedOrCasualties);
        IHtmlCollection<IElement> rows = afterDoc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        HarmedOrCasualty HarmedOrCasualty = CreateHarmedOrCasualty("Ferido");
        dbContext.HarmedOrCasualties.Add(HarmedOrCasualty);
        await dbContext.SaveChangesAsync();
        var id = HarmedOrCasualty.CasualtyId;

        IDocument listDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/HarmedOrCasualty/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["CasualtyId"] = "-1",
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/HarmedOrCasualty/List");

        // Assert
        Assert.Single(dbContext.HarmedOrCasualties);
        IElement? row = afterDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);
        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal("Ferido", cell.TextContent.Trim());
    }

    public async Task InitializeAsync()
    {
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.RemoveRange(dbContext.HarmedOrCasualties);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests.SectorTest;

public class SectorIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Theory]
    [MemberData(nameof(SectorTestData.ListScenario), MemberType = typeof(SectorTestData))]
    public async Task List_ReturnsExpectedItems(Sector[] scenarioSectors)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Sectors.AddRange(scenarioSectors);
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync("/Sector/List");

        // Assert
        DbSet<Sector> dbItems = dbContext.Sectors;
        Assert.Equal(scenarioSectors.Length, dbItems.Count());

        Assert.All(dbItems, dbItem =>
        {
            IElement? row = doc.QuerySelector($"table tbody tr[data-id='{dbItem.SectorId}']");
            Assert.NotNull(row);
            Sector scenarioSector = scenarioSectors.First(s => s.SectorName == dbItem.SectorName);

            Assert.Equal(scenarioSector.Enable, dbItem.Enable);
            Assert.Equal(row.HasAttribute("data-enable"), dbItem.Enable);

            IElement? nameCell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(nameCell);
            Assert.Equal(scenarioSector.SectorName, nameCell.TextContent.Trim());

            IElement? codeCell = row.QuerySelector("td[data-property='code']");
            Assert.NotNull(codeCell);
            Assert.Equal(scenarioSector.SectorCode, dbItem.SectorCode);
            Assert.Equal(scenarioSector.SectorCode, codeCell.TextContent.Trim());
        });
    }

    [Theory]
    [MemberData(nameof(SectorTestData.CreateScenario), MemberType = typeof(SectorTestData))]
    public async Task Create_Post_CreatesExpectedItems(Sector[] scenarioSectors)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //Act
        foreach (Sector sector in scenarioSectors)
        {
            var formData = new Dictionary<string, string>
            {
                ["SectorName"] = sector.SectorName,
                ["SectorCode"] = sector.SectorCode,
                ["Enable"] = sector.Enable.ToString()
            };

            await _client.PostAsync("/Sector/Create", new FormUrlEncodedContent(formData));
        }

        // Assert
        DbSet<Sector> dbItems = dbContext.Sectors;
        Assert.Equal(scenarioSectors.Length, dbItems.Count());
        IDocument listDoc = await _client.GetDocumentAsync("/Sector/List");

        Assert.All(dbItems, dbItem =>
        {
            IElement? row = listDoc.QuerySelector($"table tbody tr[data-id='{dbItem.SectorId}']");
            Assert.NotNull(row);
            Sector scenarioSector = scenarioSectors.First(s => s.SectorName == dbItem.SectorName);

            Assert.Equal(scenarioSector.Enable, dbItem.Enable);
            Assert.Equal(row.HasAttribute("data-enable"), dbItem.Enable);

            IElement? nameCell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(nameCell);
            Assert.Equal(scenarioSector.SectorName, nameCell.TextContent.Trim());

            IElement? codeCell = row.QuerySelector("td[data-property='code']");
            Assert.NotNull(codeCell);
            Assert.Equal(scenarioSector.SectorCode, dbItem.SectorCode);
            Assert.Equal(scenarioSector.SectorCode, codeCell.TextContent.Trim());
        });
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Sector sector = SectorTestData.CreateSector("CPLM", "CCF", true);

        dbContext.Sectors.Add(sector);
        var id = sector.SectorId;
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/Sector/Edit/{id}");

        // Assert
        Assert.Single(dbContext.Sectors);
        IElement? form = doc.QuerySelector("form[action^='/Sector/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=SectorId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=SectorName]")!;
        Assert.Equal("CPLM", nameInput.GetAttribute("value"));

        IElement codeInput = form.QuerySelector("input[name=SectorCode]")!;
        Assert.Equal("CCF", codeInput.GetAttribute("value"));

        IElement enableInput = form.QuerySelector("input[name=Enable]")!;
        var enableInputValue = enableInput.GetAttribute("value");
        Assert.NotNull(enableInputValue);
        Assert.True(bool.Parse(enableInputValue));
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Sector Sector = SectorTestData.CreateSector("CPLM", "CCF", true);

        dbContext.Sectors.Add(Sector);
        var id = Sector.SectorId;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/Sector/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/Sector/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["SectorId"] = id.ToString(),
            ["SectorName"] = "Atualizado",
            ["SectorCode"] = "ACT",
            ["Enable"] = "false"
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        await _client.PostAsync(action, content);
        IDocument listDoc = await _client.GetDocumentAsync("/Sector/List");

        //Assert
        Assert.Single(dbContext.Sectors);
        IElement? row = listDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);

        Assert.False(row.HasAttribute("data-enable"));

        IElement? nameCell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(nameCell);
        Assert.Equal("Atualizado", nameCell.TextContent.Trim());

        IElement? codeCell = row.QuerySelector("td[data-property='code']");
        Assert.NotNull(codeCell);
        Assert.Equal("ACT", codeCell.TextContent.Trim());
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var sectorName = "CPLM";
        var sectorCode = "CCF";
        Sector sector = SectorTestData.CreateSector(sectorName, sectorCode, true);

        dbContext.Sectors.Add(sector);
        var id = sector.SectorId;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/Sector/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/Sector/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["SectorId"] = id.ToString(),
            ["SectorName"] = string.Empty
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument doc = await _client.GetDocumentAsync("/Sector/List");

        // Assert
        Assert.Single(dbContext.Sectors);

        IElement? row = doc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);

        Assert.True(row.HasAttribute("data-enable"));

        IElement? nameCell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(nameCell);
        Assert.Equal(sectorName, nameCell.TextContent.Trim());

        IElement? codeCell = row.QuerySelector("td[data-property='code']");
        Assert.NotNull(codeCell);
        Assert.Equal(sectorCode, codeCell.TextContent.Trim());
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Sector Sector = SectorTestData.CreateSector("CPLM", "CCF", true);
        dbContext.Sectors.Add(Sector);
        var id = Sector.SectorId;
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/Sector/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/Sector/Delete']")!;

        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        // 2) POST the form with the correct Id & token
        var fields = new Dictionary<string, string?>
        {
            ["SectorId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/Sector/Delete/{id}", new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/Sector/List");

        // Assert 
        Assert.Empty(dbContext.Sectors);
        IElement? row = afterDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.Null(row);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Sector Sector = SectorTestData.CreateSector("CPLM", "CCF", true);
        dbContext.Sectors.Add(Sector);
        var id = Sector.SectorId;
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/Sector/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/Sector/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["SectorId"] = "-1",
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/Sector/List");

        // Assert
        Assert.Single(dbContext.Sectors);
        IElement? row = afterDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);

        Assert.True(row.HasAttribute("data-enable"));

        IElement? cell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(cell);
        Assert.Equal("CPLM", cell.TextContent.Trim());

        IElement? codeCell = row.QuerySelector("td[data-property='code']");
        Assert.NotNull(codeCell);
        Assert.Equal("CCF", codeCell.TextContent.Trim());
    }


    public async Task InitializeAsync()
    {
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.RemoveRange(dbContext.Sectors);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Tests.TestHelpers;

namespace Processos_Juridicos.Tests.UnitTest;

public class UnitIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Theory]
    [MemberData(nameof(UnitTestData.ListScenario), MemberType = typeof(UnitTestData))]
    public async Task List_ReturnsExpectedItems(UnitTest[] scenarioUnits)
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.AddRange(scenarioUnits.Select(CreateUnit()));

        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync("/Unit/List");

        // Assert
        DbSet<Unit> dbItems = dbContext.Units;
        Assert.Equal(scenarioUnits.Length, dbContext.Units.Count());

        Assert.All(dbItems, dbItem =>
        {
            IElement? row = doc.QuerySelector($"table tbody tr[data-id='{dbItem.UnitId}']");
            Assert.NotNull(row);

            Assert.Equal(row.HasAttribute("data-enable"), dbItem.Enable);

            IElement? nameCell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(nameCell);
            Assert.Equal(nameCell.TextContent.Trim(), dbItem.UnitName);

            IElement? codeCell = row.QuerySelector("td[data-property='code']");
            Assert.NotNull(codeCell);
            Assert.Equal(codeCell.TextContent.Trim(), dbItem.UnitCode);

            IElement? acronymCell = row.QuerySelector("td[data-property='acronym']");
            Assert.NotNull(acronymCell);
            Assert.Equal(acronymCell.TextContent.Trim(), dbItem.UnitAcronym);
        });
    }

    [Theory]
    [MemberData(nameof(UnitTestData.CreateScenario), MemberType = typeof(UnitTestData))]
    public async Task Create_Post_CreatesExpectedItems(UnitTest[] scenarioUnits)
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Act
        foreach (UnitTest unitTest in scenarioUnits)
        {
            var formData = new Dictionary<string, string?>
            {
                ["UnitName"] = unitTest.Name,
                ["UnitCode"] = unitTest.Code,
                ["UnitAcronym"] = unitTest.Acronym,
                ["Enable"] = unitTest.IsEnabled.ToString()
            };
            await _client.PostAsync("/Unit/Create", new FormUrlEncodedContent(formData));
        }

        // Assert
        DbSet<Unit> dbItems = dbContext.Units;
        Assert.Equal(scenarioUnits.Length, dbItems.Count());
        IDocument listDoc = await _client.GetDocumentAsync("/Unit/List");

        Assert.All(dbItems, dbItem =>
        {
            UnitTest scenarioUnit = scenarioUnits.First(unitTest => unitTest.Name == dbItem.UnitName);

            IElement? row = listDoc.QuerySelector($"table tbody tr[data-id='{dbItem.UnitId}']");
            Assert.NotNull(row);

            Assert.Equal(dbItem.Enable, scenarioUnit.IsEnabled);
            Assert.Equal(row.HasAttribute("data-enable"), scenarioUnit.IsEnabled);

            IElement? nameCell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(nameCell);
            Assert.Equal(dbItem.UnitName, scenarioUnit.Name);
            Assert.Equal(nameCell.TextContent.Trim(), scenarioUnit.Name);

            IElement? codeCell = row.QuerySelector("td[data-property='code']");
            Assert.NotNull(codeCell);
            Assert.Equal(dbItem.UnitCode, scenarioUnit.Code);
            Assert.Equal(codeCell.TextContent.Trim(), scenarioUnit.Code);

            IElement? acronymCell = row.QuerySelector("td[data-property='acronym']");
            Assert.NotNull(acronymCell);
            Assert.Equal(dbItem.UnitAcronym, scenarioUnit.Acronym);
            Assert.Equal(acronymCell.TextContent.Trim(), scenarioUnit.Acronym);
        });
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Unit unit = CreateUnit()(new UnitTest { Name = "Name", Acronym = "N", Code = "code", IsEnabled = true });
        dbContext.Add(unit);
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/Unit/Edit/{unit.UnitId}");

        // Assert

        IElement? form = doc.QuerySelector($"main form");
        Assert.NotNull(form);
        Assert.Equal(form.GetAttribute("action"), $"/Unit/Edit/{unit.UnitId}");

        IElement? nameField = form.QuerySelector("#UnitName");
        Assert.NotNull(nameField);
        Assert.Equal(unit.UnitName, nameField.GetAttribute("value"));

        IElement? codeField = form.QuerySelector("#UnitCode");
        Assert.NotNull(codeField);
        Assert.Equal(unit.UnitCode, codeField.GetAttribute("value"));

        IElement? acronymField = form.QuerySelector("#UnitAcronym");
        Assert.NotNull(acronymField);
        Assert.Equal(unit.UnitAcronym, acronymField.GetAttribute("value"));
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Unit unit = CreateUnit()(new UnitTest { Name = "Name", Acronym = "N", Code = "code", IsEnabled = true });
        dbContext.Units.Add(unit);
        await dbContext.SaveChangesAsync();

        // Act
        var formData = new Dictionary<string, string?>
        {
            ["UnitId"] = unit.UnitId.ToString(),
            ["UnitName"] = "NewName",
            ["UnitCode"] = unit.UnitCode,
            ["UnitAcronym"] = unit.UnitAcronym,
            ["Enable"] = unit.Enable.ToString(),
        };

        await _client.PostAsync($"/Unit/Edit/{unit.UnitId}", new FormUrlEncodedContent(formData));
        IDocument listDoc = await _client.GetDocumentAsync("/Unit/List");

        // Assert
        IElement? row = listDoc.QuerySelector($"table tbody tr[data-id='{unit.UnitId}']");
        Assert.NotNull(row);
        Assert.Equal(unit.Enable, row.HasAttribute("data-enable"));

        IElement? nameCell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(nameCell);
        Assert.Equal("NewName", nameCell.TextContent.Trim());


        IElement? codeCell = row.QuerySelector("td[data-property='code']");
        Assert.NotNull(codeCell);
        Assert.Equal(unit.UnitCode, codeCell.TextContent.Trim());

        IElement? acronymCell = row.QuerySelector("td[data-property='acronym']");
        Assert.NotNull(acronymCell);
        Assert.Equal(unit.UnitAcronym, acronymCell.TextContent.Trim());
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Unit unit = CreateUnit()(new UnitTest { Name = "Name", Acronym = "N", Code = "code", IsEnabled = false });
        dbContext.Units.Add(unit);
        await dbContext.SaveChangesAsync();

        // Act
        var formData = new Dictionary<string, string?>
        {
            ["UnitName"] = string.Empty,
        };
        await _client.PostAsync($"/Unit/Edit/{unit.UnitId}", new FormUrlEncodedContent(formData));
        IDocument listDoc = await _client.GetDocumentAsync("/Unit/List");

        // Assert
        IElement? row = listDoc.QuerySelector($"table tbody tr[data-id='{unit.UnitId}']");
        Assert.NotNull(row);
        Assert.Equal(unit.Enable, row.HasAttribute("data-enable"));

        IElement? nameCell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(nameCell);
        Assert.Equal(unit.UnitName, nameCell.TextContent.Trim());


        IElement? codeCell = row.QuerySelector("td[data-property='code']");
        Assert.NotNull(codeCell);
        Assert.Equal(unit.UnitCode, codeCell.TextContent.Trim());

        IElement? acronymCell = row.QuerySelector("td[data-property='acronym']");
        Assert.NotNull(acronymCell);
        Assert.Equal(unit.UnitAcronym, acronymCell.TextContent.Trim());
    }

    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Unit unit = CreateUnit()(new UnitTest { Name = "Name", Acronym = "N", Code = "code", IsEnabled = false });
        dbContext.Units.Add(unit);
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/Unit/List");

        var token = listDoc
            .QuerySelector("#deleteForm input[name=__RequestVerificationToken]")!
            .GetAttribute("value");

        var fields = new Dictionary<string, string?>
        {
            ["UnitId"] = unit.UnitId.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/Unit/Delete/{unit.UnitId}", new FormUrlEncodedContent(fields));
        IDocument lisdDoc = await _client.GetDocumentAsync("/Unit/List");

        // Assert 
        Assert.Empty(dbContext.AccidentTypes);
        IHtmlCollection<IElement> rows = lisdDoc.QuerySelectorAll("table tbody tr");
        Assert.Empty(rows);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Unit unit = CreateUnit()(new UnitTest { Name = "Name", Acronym = "N", Code = "code", IsEnabled = false });
        dbContext.Units.Add(unit);
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/Unit/List");

        var token = listDoc
            .QuerySelector("#deleteForm input[name=__RequestVerificationToken]")!
            .GetAttribute("value");

        var fields = new Dictionary<string, string?>
        {
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/Unit/Delete/{int.MaxValue}", new FormUrlEncodedContent(fields));
        listDoc = await _client.GetDocumentAsync("/Unit/List");

        // Assert
        Assert.Single(dbContext.Units);
        IElement? row = listDoc.QuerySelector($"table tbody tr[data-id='{unit.UnitId}']");
        Assert.NotNull(row);
    }

    private static Func<UnitTest, Unit> CreateUnit()
    {
        return unitTest => new Unit
        {
            Enable = unitTest.IsEnabled,
            UnitAcronym = unitTest.Acronym!,
            UnitCode = unitTest.Code!,
            UnitName = unitTest.Name!
        };
    }

    public async Task InitializeAsync()
    {
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.RemoveRange(dbContext.Units);
        await dbContext.SaveChangesAsync();
    }
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

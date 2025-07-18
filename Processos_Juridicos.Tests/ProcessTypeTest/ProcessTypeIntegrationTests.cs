using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Tests.ProcessTypeTest;

public class ProcessTypeIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Theory]
    [MemberData(nameof(ProcessTypeTestData.ListScenario), MemberType = typeof(ProcessTypeTestData))]
    public async Task List_ReturnsExpectedItems(ProcessType[] scenarioProcessTypes)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Process_types.AddRange(scenarioProcessTypes);
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync("/ProcessType/List");

        // Assert
        DbSet<ProcessType> dbItems = dbContext.Process_types;

        Assert.All(dbItems, dbItem =>
        {
            IElement? row = doc.QuerySelector($"table tbody tr[data-id='{dbItem.ProcessTypeId}']");
            Assert.NotNull(row);

            IElement? nameCell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(nameCell);
            Assert.Equal(nameCell.TextContent.Trim(), dbItem.ProcessTypeName);

            IElement? deadlineCell = row.QuerySelector("td[data-property='deadline']");
            Assert.NotNull(deadlineCell);
            Assert.Equal(deadlineCell.TextContent.Trim(), dbItem.Deadline.ToString());
        });
    }

    [Theory]
    [MemberData(nameof(ProcessTypeTestData.CreateScenario), MemberType = typeof(ProcessTypeTestData))]
    public async Task Create_Post_CreatesExpectedItems(ProcessType[] scenarioProcessTypes)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //Act
        foreach (ProcessType processType in scenarioProcessTypes)
        {
            var formData = new Dictionary<string, string>
            {
                ["ProcessTypeName"] = processType.ProcessTypeName,
                ["Deadline"] = processType.Deadline.ToString()
            };

            await _client.PostAsync("/ProcessType/Create", new FormUrlEncodedContent(formData));
        }
        IDocument listDoc = await _client.GetDocumentAsync("/ProcessType/List");

        // Assert
        DbSet<ProcessType> dbItems = dbContext.Process_types;
        Assert.Equal(scenarioProcessTypes.Length, dbItems.Count());

        Assert.All(dbItems, dbItem =>
        {
            IElement? row = listDoc.QuerySelector($"table tbody tr[data-id='{dbItem.ProcessTypeId}']");
            Assert.NotNull(row);

            IElement? nameCell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(nameCell);
            ProcessType scenarioProcessType = scenarioProcessTypes.First(p => p.ProcessTypeName == dbItem.ProcessTypeName);

            Assert.Equal(scenarioProcessType.ProcessTypeName, dbItem.ProcessTypeName);
            Assert.Equal(nameCell.TextContent.Trim(), dbItem.ProcessTypeName);

            IElement? deadlineCell = row.QuerySelector("td[data-property='deadline']");
            Assert.NotNull(deadlineCell);
            Assert.Equal(scenarioProcessType.Deadline, dbItem.Deadline);
            Assert.Equal(deadlineCell.TextContent.Trim(), dbItem.Deadline.ToString());
        });
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ProcessType sector = ProcessTypeTestData.CreateProcessType("CPLM", 15);
        dbContext.Process_types.Add(sector);
        var id = sector.ProcessTypeId;
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/ProcessType/Edit/{id}");

        // Assert
        Assert.Single(dbContext.Process_types);
        IElement? form = doc.QuerySelector("form[action^='/ProcessType/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=ProcessTypeId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=ProcessTypeName]")!;
        Assert.Equal("CPLM", nameInput.GetAttribute("value"));

        IElement deadlineInput = form.QuerySelector("input[name=Deadline]")!;
        Assert.Equal("15", deadlineInput.GetAttribute("value"));
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        ProcessType ProcessType = ProcessTypeTestData.CreateProcessType("CPLM", 15);

        dbContext.Process_types.Add(ProcessType);
        var id = ProcessType.ProcessTypeId;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/ProcessType/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/ProcessType/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["ProcessTypeId"] = id.ToString(),
            ["ProcessTypeName"] = "Atualizado",
            ["Deadline"] = "20"
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        await _client.PostAsync(action, content);
        IDocument listDoc = await _client.GetDocumentAsync("/ProcessType/List");

        //Assert
        Assert.Single(dbContext.Process_types);
        IElement? nameCell = listDoc.QuerySelector("table tbody td[data-property='name']");
        Assert.NotNull(nameCell);
        Assert.Equal("Atualizado", nameCell.TextContent.Trim());

        IElement? deadlineCell = listDoc.QuerySelector("table tbody td[data-property='deadline']");
        Assert.NotNull(deadlineCell);
        Assert.Equal("20", deadlineCell.TextContent.Trim());
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var processTypeName = "Revisão";
        var processTypeDeadline = 15;
        ProcessType processType = ProcessTypeTestData.CreateProcessType(processTypeName, processTypeDeadline);
        dbContext.Process_types.Add(processType);
        var id = processType.ProcessTypeId;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/ProcessType/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/ProcessType/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["ProcessTypeId"] = id.ToString(),
            ["ProcessTypeName"] = string.Empty,
            ["Deadline"] = string.Empty,
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument listDoc = await _client.GetDocumentAsync("/ProcessType/List");

        // Assert
        Assert.Single(dbContext.Process_types);

        IElement? row = listDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);

        IElement? nameCell = listDoc.QuerySelector("table tbody td[data-property='name']");
        Assert.NotNull(nameCell);
        Assert.Equal(processType.ProcessTypeName, nameCell.TextContent.Trim());

        IElement? deadlineCell = listDoc.QuerySelector("table tbody td[data-property='deadline']");
        Assert.NotNull(deadlineCell);
        Assert.Equal(processTypeDeadline.ToString(), deadlineCell.TextContent.Trim());
    }



    [Fact]
    public async Task Delete_Successful_RemovesItemAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ProcessType ProcessType = ProcessTypeTestData.CreateProcessType("CPLM", 15);
        dbContext.Process_types.Add(ProcessType);
        var id = ProcessType.ProcessTypeId;
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/ProcessType/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/ProcessType/Delete']")!;

        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string?>
        {
            ["ProcessTypeId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/ProcessType/Delete/{id}", new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/ProcessType/List");

        // Assert 
        Assert.Empty(dbContext.Process_types);
        IElement? row = afterDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.Null(row);
    }

    [Fact]
    public async Task Delete_NonExistent_ItemRemainsAndRedirects()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ProcessType ProcessType = ProcessTypeTestData.CreateProcessType("CPLM", 15);
        dbContext.Process_types.Add(ProcessType);
        var id = ProcessType.ProcessTypeId;
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/ProcessType/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/ProcessType/Delete']")!;
        var action = deleteForm.GetAttribute("action")!;
        var token = deleteForm
            .QuerySelector("input[name=__RequestVerificationToken]")!
            .GetAttribute("value")!;

        var fields = new Dictionary<string, string>
        {
            ["ProcessTypeId"] = "-1",
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/ProcessType/List");

        // Assert
        Assert.Single(dbContext.Process_types);
        IElement? row = afterDoc.QuerySelector($"table tbody tr[data-id='{id}']");
        Assert.NotNull(row);

        IElement? nameCell = row.QuerySelector("td[data-property='name']");
        Assert.NotNull(nameCell);
        Assert.Equal("CPLM", nameCell.TextContent.Trim());

        IElement? deadlineCell = listDoc.QuerySelector("table tbody td[data-property='deadline']");
        Assert.NotNull(deadlineCell);
        Assert.Equal("15", deadlineCell.TextContent.Trim());
    }


    public async Task InitializeAsync()
    {
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.RemoveRange(dbContext.Process_types);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

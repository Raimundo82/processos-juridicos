using System.Diagnostics;

using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;

using Process = Processos_Juridicos.Entities.Process;

namespace Processos_Juridicos.Tests.ProcessTest;


//TODO: authentication related tests
public class ProcessIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();


    [Theory]
    [MemberData(nameof(ProcessTestData.BaseScenario), MemberType = typeof(ProcessTestData))]
    public async Task List_ReturnsExpectedItems(Process[] scenarioProcesses)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Processes.AddRange(scenarioProcesses);
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync("/Process/List");

        // Assert
        DbSet<Process> dbProcessesItems = dbContext.Processes;
        Assert.Equal(scenarioProcesses.Length, dbContext.Processes.Count());
        Assert.All(dbProcessesItems, dbItem =>
        {
            IElement? row = doc.QuerySelector($"table tbody tr[data-id='{dbItem.ProcessId}']");
            Assert.NotNull(row);

            IElement? nuipmCell = row.QuerySelector("td[data-property='nuipm']");
            Assert.NotNull(nuipmCell);
            Assert.Equal(nuipmCell.TextContent.Trim(), dbItem.Nuipm);

            IElement? stateCell = row.QuerySelector("td[data-property='state']");
            Assert.NotNull(stateCell);
            Assert.Equal(stateCell.TextContent.Trim(), dbItem.ProcessState.StateName);

            IElement? processTypeCell = row.QuerySelector($"td[data-property='process-type']");
            Assert.NotNull(processTypeCell);
            Assert.Equal(processTypeCell.TextContent.Trim(), dbItem.ProcessType.ProcessTypeName);
        });
    }


    [Theory]
    [MemberData(nameof(ProcessTestData.BaseScenario), MemberType = typeof(ProcessTestData))]
    public async Task Create_Post_CreatesExpectedItems(Process[] scenarioProcesses)
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.States.AddRange(scenarioProcesses.Select(s => s.ProcessState));
        dbContext.ProcessTypes.AddRange(scenarioProcesses.Select(s => s.ProcessType));
        await dbContext.SaveChangesAsync();

        // Act
        foreach (Process scenarioProcess in scenarioProcesses)
        {
            IDocument doc = await _client.GetDocumentAsync("/Process/Create");

            var processStateId = doc.GetElementById("state-id")?
                .QuerySelectorAll("option")
                .First(option => option.TextContent.Trim() == scenarioProcess.ProcessState.StateName)
                .GetAttribute("value");

            var processTypeId = doc.GetElementById("process-type-id")?
                .QuerySelectorAll("option")
                .First(option => option.TextContent.Trim() == scenarioProcess.ProcessType.ProcessTypeName)
                .GetAttribute("value");

            var formData = new Dictionary<string, string?>
            {
                ["Nuipm"] = scenarioProcess.Nuipm,
                ["ProcessTypeId"] = processTypeId,
                ["ProcessStateId"] = processStateId,
            };

            await _client.PostAsync("/Process/Create", new FormUrlEncodedContent(formData));
        }

        // Assert
        DbSet<Process> dbItems = dbContext.Processes;
        Assert.Equal(scenarioProcesses.Length, dbItems.Count());

        Debug.WriteLine(dbItems.Count());

        Assert.All(scenarioProcesses, scenarioProcess =>
        {
            Process? expected = dbItems.FirstOrDefault(dbItem => dbItem.Nuipm == scenarioProcess.Nuipm);
            Assert.NotNull(expected);
            Assert.Equal(expected.ProcessState.StateName, scenarioProcess.ProcessState.StateName);
            Assert.Equal(expected.ProcessType.ProcessTypeName, scenarioProcess.ProcessType.ProcessTypeName);
            Assert.Equal(expected.ProcessType.Deadline, scenarioProcess.ProcessType.Deadline);
        });

    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");

        // Assert
        IElement? form = doc.QuerySelector($"main form");
        Assert.NotNull(form);
        Assert.Equal(form.GetAttribute("action"), $"/Process/Edit/{process.ProcessId}");

        IElement? nuipmField = form.QuerySelector("#process-nuipm");
        Assert.NotNull(nuipmField);
        Assert.Equal(process.Nuipm, nuipmField.GetAttribute("value"));

        IElement? stateField = form.QuerySelector("#state-id option[selected='selected']");
        Assert.NotNull(stateField);
        Assert.Equal(stateField.GetAttribute("value"), process.ProcessStateId.ToString());
        Assert.Equal(stateField.TextContent.Trim(), process.ProcessState.StateName);

        IElement? processTypeField = form.QuerySelector($"#process-type-id option[selected='selected']");
        Assert.NotNull(processTypeField);
        Assert.Equal(processTypeField.GetAttribute("value"), process.ProcessTypeId.ToString());
        Assert.Equal(processTypeField.TextContent.Trim(), process.ProcessType.ProcessTypeName);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_Updates()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ProcessState newState = dbContext.Add(new ProcessState { StateName = "NewState" }).Entity;
        ProcessType newType = dbContext.Add(new ProcessType { ProcessTypeName = "NewType", Deadline = 15 }).Entity;
        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();
        dbContext.Entry(process).State = EntityState.Detached;

        IDocument doc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");
        var newTypeId = doc.GetElementById("process-type-id")?.QuerySelector($"option[value='{newType.ProcessTypeId}']")?.GetAttribute("value");
        var newStateId = doc.GetElementById("state-id")?.QuerySelector($"option[value='{newState.ProcessStateId}']")?.GetAttribute("value");
        var token = doc.QuerySelector("input[name=__RequestVerificationToken]")?.GetAttribute("value")!;

        // Act
        var formData = new Dictionary<string, string?>
        {
            ["ProcessId"] = process.ProcessId.ToString(),
            ["Nuipm"] = "NewNuipm",
            ["ProcessTypeId"] = newTypeId,
            ["ProcessStateId"] = newStateId,
            ["__RequestVerificationToken"] = token
        };

        await _client.PostAsync($"/Process/Edit/{process.ProcessId}", new FormUrlEncodedContent(formData));

        // Assert
        DbSet<Process> dbItems = dbContext.Processes;
        Assert.Single(dbItems);
        Process underTest = dbItems.First();

        Assert.Equal("NewNuipm", underTest.Nuipm);
        Assert.Equal(newState.ProcessStateId, underTest.ProcessStateId);
        Assert.Equal(newState.StateName, underTest.ProcessState.StateName);
        Assert.Equal(newType.ProcessTypeId, underTest.ProcessTypeId);
        Assert.Equal(newType.ProcessTypeName, underTest.ProcessType.ProcessTypeName);
        Assert.Equal(newType.Deadline, underTest.ProcessType.Deadline);
    }


    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();
        dbContext.Entry(process).State = EntityState.Detached;

        // Act
        var formData = new Dictionary<string, string?>
        {
            ["Nuipm"] = string.Empty,
        };

        await _client.PostAsync($"/Process/Edit/{process.ProcessId}", new FormUrlEncodedContent(formData));

        // Assert
        DbSet<Process> dbItems = dbContext.Processes;
        Assert.Single(dbItems);
        Process underTest = dbItems.First();

        Assert.Equal(process.Nuipm, underTest.Nuipm);
        Assert.Equal(process.ProcessStateId, underTest.ProcessStateId);
        Assert.Equal(process.ProcessTypeId, underTest.ProcessTypeId);
        Assert.Equal(process.ProcessState.StateName, underTest.ProcessState.StateName);
        Assert.Equal(process.ProcessType.ProcessTypeName, underTest.ProcessType.ProcessTypeName);
    }


    [Fact]
    public async Task Delete_Successful_RemovesItem()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/Process/List");

        var token = listDoc
            .QuerySelector("form[action='/Process/Delete'] input[name=__RequestVerificationToken]")!
            .GetAttribute("value");

        var fields = new Dictionary<string, string?>
        {
            ["ProcessId"] = process.ProcessId.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/Process/Delete/{process.ProcessId}", new FormUrlEncodedContent(fields));

        // Assert 
        DbSet<Process> dbItems = dbContext.Processes;
        Assert.Empty(dbItems);
    }


    [Fact]
    public async Task Delete_InvalidModel_ItemRemains()
    {
        // Arrange
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/Process/List");
        IElement deleteForm = listDoc.QuerySelector("form[action='/Process/Delete']")!;

        var token = listDoc
            .QuerySelector("form[action='/Process/Delete'] input[name=__RequestVerificationToken]")!
            .GetAttribute("value");

        var fields = new Dictionary<string, string?>
        {
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/Process/Delete/{int.MaxValue}", new FormUrlEncodedContent(fields));

        // Assert
        Assert.Single(dbContext.Processes);
    }

    private static Process CreateProcess()
    {
        return new Process
        {
            Nuipm = "1234",
            ProcessType = new ProcessType { Deadline = 15, ProcessTypeName = "Tipo 1" },
            ProcessState = new ProcessState { StateName = "State 1" }
        };
    }

    public async Task InitializeAsync()
    {
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.RemoveRange(dbContext.Processes);
        dbContext.RemoveRange(dbContext.ProcessTypes);
        dbContext.RemoveRange(dbContext.States);
        dbContext.RemoveRange(dbContext.ProcessFiles);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

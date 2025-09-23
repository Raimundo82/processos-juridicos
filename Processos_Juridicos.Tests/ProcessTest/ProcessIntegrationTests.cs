using System.Net;
using System.Net.Http.Headers;

using AngleSharp.Dom;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Tests.TestHelpers;

using Process = Processos_Juridicos.Entities.Process;

namespace Processos_Juridicos.Tests.ProcessTest;


//TODO: authentication related tests
public class ProcessIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        HandleCookies = true
    });


    [Theory]
    [MemberData(nameof(ProcessTestData.BaseScenario), MemberType = typeof(ProcessTestData))]
    public async Task List_ReturnsExpectedItems(Process[] scenarioProcesses)
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];

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

    [Fact]
    public async Task Details_Get_ShowsExpectedProcessFields()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/Process/Details/{process.ProcessId}");

        // Assert
        IElement? mainDl = doc.QuerySelector("dl.main-data");
        Assert.NotNull(mainDl);

        IElement? nuipmDd = mainDl.QuerySelectorAll("dt")
            .FirstOrDefault(dt => dt.TextContent.Trim() == "NUIPM")?.NextElementSibling;
        Assert.NotNull(nuipmDd);
        Assert.Equal(process.Nuipm, nuipmDd.TextContent.Trim());

        IElement? processTypeDd = mainDl.QuerySelectorAll("dt")
            .FirstOrDefault(dt => dt.TextContent.Trim() == "Tipo de Processo")?.NextElementSibling;
        Assert.NotNull(processTypeDd);
        Assert.Equal(process.ProcessType.ProcessTypeName, processTypeDd.TextContent.Trim());

        IElement? stateDd = mainDl.QuerySelectorAll("dt")
            .FirstOrDefault(dt => dt.TextContent.Trim() == "Estado")?.NextElementSibling;
        Assert.NotNull(stateDd);
        Assert.Equal(process.ProcessState.StateName, stateDd.TextContent.Trim());
    }

    [Fact]
    public async Task Details_Get_WithInvalidIdParameter_GoesToListPage()
    {
        // Act
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        HttpResponseMessage response = await _client.GetAsync($"/Process/Details/abc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Processos", content);
    }

    [Fact]
    public async Task Details_Get_WhenProcessDoesNotExist_RedirectsToList()
    {
        // Act
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        HttpResponseMessage response = await _client.GetAsync($"/Process/Details/9999");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Processos", content);
    }

    [Theory]
    [MemberData(nameof(ProcessTestData.BaseScenario), MemberType = typeof(ProcessTestData))]
    public async Task Create_Post_CreatesExpectedItems(Process[] scenarioProcesses)
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];

        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.States.AddRange(scenarioProcesses.Select(s => s.ProcessState));
        dbContext.ProcessTypes.AddRange(scenarioProcesses.Select(s => s.ProcessType));

        dbContext.Units.AddRange(scenarioProcesses.Select(s => s.Unit));

        await dbContext.SaveChangesAsync();

        // Act
        foreach (Process scenarioProcess in scenarioProcesses)
        {
            IDocument doc = await _client.GetDocumentAsync("/Process/Create");

            var processStateId = doc.QuerySelector("select[name=ProcessStateId]")?
                .QuerySelectorAll("option")
                .FirstOrDefault()?
                .GetAttribute("value");

            var unitId = doc.QuerySelector("select[name=UnitId]")?
                .QuerySelectorAll("option")
                .FirstOrDefault(option => option.TextContent.Trim() == scenarioProcess.Unit.UnitName)?
                .GetAttribute("value");

            var processTypeId = doc.QuerySelector("select[name=ProcessTypeId]")?
                .QuerySelectorAll("option")
                .FirstOrDefault(option => option.TextContent.Trim() == scenarioProcess.ProcessType.ProcessTypeName)?
                .GetAttribute("value");

            var formData = new Dictionary<string, string?>
            {
                ["ProcessTypeId"] = processTypeId,
                ["ProcessStateId"] = processStateId,
                ["UnitId"] = unitId,
                ["CreatedAt"] = scenarioProcess.CreatedAt.ToString()
            };

            await _client.PostAsync("/Process/Create", new FormUrlEncodedContent(formData));
        }

        // Assert
        DbSet<Process> dbItems = dbContext.Processes;
        Assert.Equal(scenarioProcesses.Length, dbItems.Count());

        Assert.All(scenarioProcesses, scenarioProcess =>
        {
            Process? expected = dbItems.FirstOrDefault(dbItem =>
                dbItem.ProcessType.ProcessTypeName == scenarioProcess.ProcessType!.ProcessTypeName &&
                dbItem.Unit.UnitName == scenarioProcess.Unit.UnitName);

            Assert.NotNull(expected);
            Assert.True(
                expected.ProcessState.StateName is "Em Edição" or
                "Em Validação",
                $"StateName was '{expected.ProcessState.StateName}', expected 'Em Edição' or 'Em Validação'."
            );

            Assert.Equal(expected.ProcessType.ProcessTypeName, scenarioProcess.ProcessType.ProcessTypeName);
            Assert.Equal(expected.ProcessType.Deadline, scenarioProcess.ProcessType.Deadline);
        });
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");

        // Assert
        IElement? form = doc.QuerySelector("main form");
        Assert.NotNull(form);
        Assert.Equal($"/Process/Edit/{process.ProcessId}", form.GetAttribute("action"));

        IElement? nuipmField = form.QuerySelector("input[name=Nuipm]");
        Assert.NotNull(nuipmField);
        Assert.Equal(process.Nuipm, nuipmField.GetAttribute("value"));

        IElement? stateField = form.QuerySelector("select[name=ProcessStateId] option[selected]");
        Assert.NotNull(stateField);
        Assert.Equal(process.ProcessStateId.ToString(), stateField.GetAttribute("value"));
        Assert.Equal(process.ProcessState.StateName, stateField.TextContent.Trim());

        IElement? processTypeField = form.QuerySelector("select[name=ProcessTypeId] option[selected]");
        Assert.NotNull(processTypeField);
        Assert.Equal(process.ProcessTypeId.ToString(), processTypeField.GetAttribute("value"));
        Assert.Equal(process.ProcessType.ProcessTypeName, processTypeField.TextContent.Trim());
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_Updates()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];

        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ProcessState targetState = dbContext.Add(new ProcessState { StateName = "OldState" }).Entity;

        ProcessType newType = dbContext.Add(new ProcessType { ProcessTypeName = "NewType", Deadline = 15 }).Entity;

        Unit newUnit = dbContext.Add(new Unit { UnitName = "Unidade 1", UnitCode = "UN01", UnitAcronym = "UN1" }).Entity;

        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();
        dbContext.Entry(process).State = EntityState.Detached;

        StateTransition transition = dbContext.Add(new StateTransition
        {
            FromStateId = process.ProcessStateId,
            ToStateId = targetState.ProcessStateId
        }).Entity;

        Role role = dbContext.Add(new Role { RoleName = "DJ-AUTHORIZED" }).Entity;
        dbContext.Add(new StateTransitionRole
        {
            StateTransition = transition,
            RoleId = (int)role.RoleId!
        });

        await dbContext.SaveChangesAsync();

        IDocument doc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");

        var newTypeId = doc.QuerySelector("select[name=ProcessTypeId]")?
            .QuerySelector($"option[value='{newType.ProcessTypeId}']")?
            .GetAttribute("value");

        var newStateId = doc.QuerySelector("select[name=ProcessStateId]")?
            .QuerySelector($"option[value='{targetState.ProcessStateId}']")?
            .GetAttribute("value");

        var newUnitId = doc.QuerySelector("select[name=UnitId]")?
            .QuerySelector($"option[value='{newUnit.UnitId}']")?
            .GetAttribute("value");

        var token = doc.QuerySelector("input[name=__RequestVerificationToken]")?.GetAttribute("value")!;

        // Act
        var formData = new Dictionary<string, string?>
        {
            ["ProcessId"] = process.ProcessId.ToString(),
            ["ProcessTypeId"] = newTypeId,
            ["ProcessStateId"] = newStateId,
            ["UnitId"] = newUnitId,
            ["__RequestVerificationToken"] = token
        };

        await _client.PostAsync($"/Process/Edit/{process.ProcessId}", new FormUrlEncodedContent(formData));

        // Assert
        DbSet<Process> dbItems = dbContext.Processes;
        Assert.Single(dbItems);
        Process underTest = dbItems.First();

        Assert.Equal(targetState.ProcessStateId, underTest.ProcessStateId);
        Assert.Equal(targetState.StateName, underTest.ProcessState.StateName);
        Assert.Equal(newUnit.UnitId, underTest.Unit.UnitId);

        Assert.Equal(newType.ProcessTypeId, underTest.ProcessTypeId);
        Assert.Equal(newType.ProcessTypeName, underTest.ProcessType.ProcessTypeName);
        Assert.Equal(newType.Deadline, underTest.ProcessType.Deadline);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_DoesNotApplyChanges()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
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
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
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
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Add(CreateProcess());
        await dbContext.SaveChangesAsync();

        IDocument listDoc = await _client.GetDocumentAsync("/Process/List");

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

    [Fact]
    public async Task Edit_GetAndDownload_ObtainsFile()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        var pdfBytes = CreatePdfBytes("DummyPDF.pdf");
        var fileEntity = new ProcessFile
        {
            Process = process,
            ProcessFileContent = pdfBytes,
            ProcessFileName = "document.pdf",
            ProcessFileType = "application/pdf"
        };

        dbContext.ProcessFiles.Add(fileEntity);
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");

        // Assert
        var rowSelector = $"#file-row-{fileEntity.ProcessFileId}";
        IElement? row = doc.QuerySelector(rowSelector);
        Assert.NotNull(row);

        IElement downloadLink = row.QuerySelector("#download-file")!;
        var href = downloadLink.GetAttribute("href")!;
        Assert.Contains($"/ProcessFile/DownloadFile", href);
        Assert.Contains(fileEntity.ProcessFileId.ToString()!, href);

        HttpResponseMessage downloadResp = await _client.GetAsync(href);
        Assert.True(downloadResp.IsSuccessStatusCode);
        Assert.Equal("application/pdf",
                    downloadResp.Content.Headers.ContentType!.MediaType);

        var gotBytes = await downloadResp.Content.ReadAsByteArrayAsync();
        Assert.Equal(pdfBytes, gotBytes);
    }


    [Fact]
    public async Task Edit_Post_WhenModelIsValidAndFileUploaded_PersistsFile()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];

        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");
        IElement form = editDoc.QuerySelector("form[action^='/Process/Edit']")!;
        var action = form.GetAttribute("action")!;

        var allNames = editDoc
          .QuerySelectorAll("form [name]")
          .Select(e => e.GetAttribute("name")!)
          .Distinct()
          .ToList();

        var pdfBytes = CreatePdfBytes("DummyPDF.pdf");
        var pdfContent = new ByteArrayContent(pdfBytes);
        pdfContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        pdfContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"ProcessFiles\"",
            FileName = "\"document.pdf\"",
        };

        var multipart = new MultipartFormDataContent();

        foreach (var name in allNames)
        {
            IElement input = editDoc.QuerySelector($"[name='{name}']")!;
            var val = input.GetAttribute("value") ?? string.Empty;

            switch (name)
            {
                case "Nuipm":
                    val = "Atualizado";
                    break;
                case "ProcessStateId":
                    val = process.ProcessStateId.ToString();
                    break;
                case "ProcessTypeId":
                    val = process.ProcessTypeId.ToString();
                    break;
                case "CreatedBy":
                    val = process.CreatedByName.ToString();
                    break;
                default:
                    break;
            }

            multipart.Add(new StringContent(val!), name);
        }

        multipart.Add(pdfContent, "ProcessFiles", "document.pdf");

        // Act
        HttpResponseMessage response = await _client.PostAsync(action, multipart);
        response.EnsureSuccessStatusCode();

        // Assert
        ProcessFile? savedFile = await dbContext.ProcessFiles.SingleOrDefaultAsync(f => f.ProcessFileName == "document.pdf");
        Assert.NotNull(savedFile);

        Assert.Equal("application/pdf", savedFile.ProcessFileType);
        Assert.Equal(pdfBytes, savedFile.ProcessFileContent);
    }

    [Fact]
    public async Task Edit_Post_FileUploadedHasUnallowedExtension_FileDoesNotPersist()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");
        IElement form = editDoc.QuerySelector("form[action^='/Process/Edit']")!;
        var action = form.GetAttribute("action")!;

        var allNames = editDoc
          .QuerySelectorAll("form [name]")
          .Select(e => e.GetAttribute("name")!)
          .Distinct()
          .ToList();

        var fileBytes = CreatePdfBytes("TextFile.txt");
        var pdfContent = new ByteArrayContent(fileBytes);
        pdfContent.Headers.ContentType = new MediaTypeHeaderValue("application/txt");
        pdfContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"ProcessFiles\"",
            FileName = "\"document.txt\"",
        };

        var multipart = new MultipartFormDataContent();

        foreach (var name in allNames)
        {
            IElement input = editDoc.QuerySelector($"[name='{name}']")!;
            var val = input.GetAttribute("value") ?? string.Empty;

            switch (name)
            {
                case "Nuipm":
                    val = "Atualizado";
                    break;
                case "ProcessStateId":
                    val = process.ProcessStateId.ToString();
                    break;
                case "ProcessTypeId":
                    val = process.ProcessTypeId.ToString();
                    break;
                case "CreatedBy":
                    val = process.CreatedByName.ToString();
                    break;
                default:
                    break;
            }

            multipart.Add(new StringContent(val!), name);
        }

        multipart.Add(pdfContent, "ProcessFiles", "document.pdf");

        // Act
        HttpResponseMessage response = await _client.PostAsync(action, multipart);
        response.EnsureSuccessStatusCode();

        // Assert
        ProcessFile? savedFile = await dbContext.ProcessFiles
           .SingleOrDefaultAsync(f => f.ProcessFileName == "TextFile.txt");

        Assert.Null(savedFile);
    }

    [Fact]
    public async Task Edit_Post_WhenEmptyFileUploaded_DoesNotPersistFile()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");
        IElement form = editDoc.QuerySelector("form[action^='/Process/Edit']")!;
        var action = form.GetAttribute("action")!;

        var allNames = editDoc
          .QuerySelectorAll("form [name]")
          .Select(e => e.GetAttribute("name")!)
          .Distinct()
          .ToList();

        var emptyBytes = Array.Empty<byte>();
        var emptyContent = new ByteArrayContent(emptyBytes);
        emptyContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        emptyContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"ProcessFiles\"",
            FileName = "\"empty.pdf\"",
        };

        var multipart = new MultipartFormDataContent();
        foreach (var name in allNames)
        {
            IElement input = editDoc.QuerySelector($"[name='{name}']")!;
            var val = input.GetAttribute("value") ?? string.Empty;

            switch (name)
            {
                case "Nuipm":
                    val = "Atualizado";
                    break;
                case "ProcessStateId":
                    val = process.ProcessStateId.ToString();
                    break;
                case "ProcessTypeId":
                    val = process.ProcessTypeId.ToString();
                    break;
                case "CreatedBy":
                    val = process.CreatedByName.ToString();
                    break;
                default:
                    break;
            }

            multipart.Add(new StringContent(val!), name);
        }

        multipart.Add(emptyContent, "ProcessFiles", "empty.pdf");

        // Act
        HttpResponseMessage response = await _client.PostAsync(action, multipart);
        response.EnsureSuccessStatusCode();

        // Assert
        ProcessFile? savedFile = await dbContext.ProcessFiles
            .SingleOrDefaultAsync(f => f.ProcessFileName == "empty.pdf");

        Assert.Null(savedFile);
    }

    [Fact]
    public async Task Edit_Post_WhenFileTooLargeUploaded_DoesNotPersistFile()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");
        IElement form = editDoc.QuerySelector("form[action^='/Process/Edit']")!;
        var action = form.GetAttribute("action")!;

        var allNames = editDoc
          .QuerySelectorAll("form [name]")
          .Select(e => e.GetAttribute("name")!)
          .Distinct()
          .ToList();

        var largeBytes = CreatePdfBytes("LargeFile.pdf");
        var largeContent = new ByteArrayContent(largeBytes);
        largeContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        largeContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"ProcessFiles\"",
            FileName = "\"largefile.pdf\"",
        };

        var multipart = new MultipartFormDataContent();
        foreach (var name in allNames)
        {
            IElement input = editDoc.QuerySelector($"[name='{name}']")!;
            var val = input.GetAttribute("value") ?? string.Empty;

            switch (name)
            {
                case "Nuipm":
                    val = "Atualizado";
                    break;
                case "ProcessStateId":
                    val = process.ProcessStateId.ToString();
                    break;
                case "ProcessTypeId":
                    val = process.ProcessTypeId.ToString();
                    break;
                case "CreatedBy":
                    val = process.CreatedByName.ToString();
                    break;
                default:
                    break;
            }

            multipart.Add(new StringContent(val!), name);
        }

        multipart.Add(largeContent, "ProcessFiles", "largefile.pdf");

        // Act
        HttpResponseMessage response = await _client.PostAsync(action, multipart);
        response.EnsureSuccessStatusCode();

        //Assert
        ProcessFile? savedFile = await dbContext.ProcessFiles
            .SingleOrDefaultAsync(f => f.ProcessFileName == "largefile.pdf");

        Assert.Null(savedFile);
    }

    [Fact]
    public async Task Edit_Post_WhenFileSignatureUnallowed_DoesNotPersistFile()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Process process = dbContext.Add(CreateProcess()).Entity;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");
        IElement form = editDoc.QuerySelector("form[action^='/Process/Edit']")!;
        var action = form.GetAttribute("action")!;

        var allNames = editDoc
          .QuerySelectorAll("form [name]")
          .Select(e => e.GetAttribute("name")!)
          .Distinct()
          .ToList();

        var fakeBytes = CreatePdfBytes("FakePdf.pdf");
        var fakeContent = new ByteArrayContent(fakeBytes);
        fakeContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        fakeContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"ProcessFiles\"",
            FileName = "\"fakepdf.pdf\"",
        };

        var multipart = new MultipartFormDataContent();
        foreach (var name in allNames)
        {
            IElement input = editDoc.QuerySelector($"[name='{name}']")!;
            var val = input.GetAttribute("value") ?? string.Empty;

            switch (name)
            {
                case "Nuipm":
                    val = "Atualizado";
                    break;
                case "ProcessStateId":
                    val = process.ProcessStateId.ToString();
                    break;
                case "ProcessTypeId":
                    val = process.ProcessTypeId.ToString();
                    break;
                case "CreatedBy":
                    val = process.CreatedByName.ToString();
                    break;
                default:
                    break;
            }

            multipart.Add(new StringContent(val!), name);
        }

        multipart.Add(fakeContent, "ProcessFiles", "fakepdf.pdf");

        // Act
        HttpResponseMessage response = await _client.PostAsync(action, multipart);
        response.EnsureSuccessStatusCode();

        // Assert
        ProcessFile? savedFile = await dbContext.ProcessFiles
            .SingleOrDefaultAsync(f => f.ProcessFileName == "fakepdf.pdf");

        Assert.Null(savedFile);
    }



    [Fact]
    public async Task Edit_GetThenDelete_DeletesFile()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];

        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Process process = dbContext.Add(CreateProcess()).Entity;

        var initialProcessFileCount = dbContext.ProcessFiles.Count();

        var pdfBytes = CreatePdfBytes("DummyPDF.pdf");

        var fileEntity = new ProcessFile
        {
            Process = process,
            ProcessFileContent = pdfBytes,
            ProcessFileName = "document.pdf",
            ProcessFileType = "application/pdf"
        };

        dbContext.ProcessFiles.Add(fileEntity);
        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();
        ProcessFile loaded = dbContext.ProcessFiles
            .Include(f => f.Process)
            .First(f => f.ProcessFileId == fileEntity.ProcessFileId);


        // Load edit page
        IDocument doc = await _client.GetDocumentAsync($"/Process/Edit/{process.ProcessId}");
        IElement? form = doc.QuerySelector("form[action^='/Process/Edit']");
        Assert.NotNull(form);

        var action = form!.GetAttribute("action")!;

        // Find the file row and prepare deletion
        var rowSelector = $"#file-row-{fileEntity.ProcessFileId}";
        IElement? row = doc.QuerySelector(rowSelector);
        Assert.NotNull(row);

        row = doc.QuerySelectorAll("div[id^='file-row-']").First();

        IElement container = doc.QuerySelector("#deletedFilesContainer")!;
        var deleteId = row.Id!;
        var deleteIdSplit = deleteId.Split('-');
        var number = deleteIdSplit[^1];

        // Add hidden input FilesToRemove
        IElement hidden = doc.CreateElement("input");
        hidden.SetAttribute("type", "hidden");
        hidden.SetAttribute("name", "FilesToRemove");
        hidden.SetAttribute("value", number);
        container.AppendChild(hidden);

        // Remove the row from DOM (simulating UI deletion)
        row.Remove();

        // Build multipart from all form fields
        var multipart = new MultipartFormDataContent();

        var allNames = doc
          .QuerySelectorAll("form [name]")
          .Select(e => e.GetAttribute("name")!)
          .Distinct()
          .ToList();


        foreach (var name in allNames)
        {
            IElement input = doc.QuerySelector($"[name='{name}']")!;
            var val = input.GetAttribute("value") ?? string.Empty;

            switch (name)
            {
                case "ProcessStateId":
                    val = process.ProcessStateId.ToString(); // ensure it's a valid int
                    break;
                case "Infringements":
                    // If process.Infringements is List<int>
                    foreach (Infringement? id in process.Infringements)
                    {
                        multipart.Add(new StringContent(id.ToString()!), "Infringements");
                    }
                    break;
                default:
                    break;
            }

            multipart.Add(new StringContent(val), name);
        }

        if (!allNames.Contains("FilesToRemove"))
        {
            multipart.Add(new StringContent(number), "FilesToRemove");
        }

        var postUrl = new Uri(_client.BaseAddress!, action);

        using var request = new HttpRequestMessage(HttpMethod.Post, postUrl)
        {
            Content = multipart
        };


        // Act
        HttpResponseMessage response = await _client.SendAsync(request);

        // Assert
        dbContext.ChangeTracker.Clear(); // ensure we don't read cached entities
        var finalCount = dbContext.ProcessFiles.Count();

        Assert.Equal(initialProcessFileCount, finalCount);
    }

    // Helper handler to log outgoing request body
    public class LoggingHandler(HttpMessageHandler innerHandler) : DelegatingHandler(innerHandler)
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content != null)
            {
                await request.Content.ReadAsStringAsync(cancellationToken);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }


    private static Process CreateProcess()
    {
        return new Process
        {
            Nuipm = "0001/" + DateTime.Now.Year.ToString() + "/",
            ProcessType = new ProcessType { Deadline = 15, ProcessTypeName = "Tipo 1" },
            ProcessState = new ProcessState { StateName = "State 1" }
        };

    }
    private static byte[] CreatePdfBytes(string fileName)
    {
        var baseDir = AppContext.BaseDirectory;
        var pdfPath = Path.Combine(baseDir, "TestFiles", fileName);

        return !File.Exists(pdfPath) ? throw new FileNotFoundException("Test PDF not found", pdfPath) : File.ReadAllBytes(pdfPath);
    }



    public async Task InitializeAsync()
    {
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.RemoveRange(dbContext.Processes);
        dbContext.RemoveRange(dbContext.ProcessTypes);
        dbContext.RemoveRange(dbContext.States);
        dbContext.RemoveRange(dbContext.ProcessFiles);
        dbContext.RemoveRange(dbContext.Units);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

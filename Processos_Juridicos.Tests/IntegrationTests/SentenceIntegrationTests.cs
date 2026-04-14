using AngleSharp.Dom;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Data;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Tests.TestHelpers;

namespace Processos_Juridicos.Tests.IntegrationTests;

public class SentenceIntegrationTests(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>,
    IAsyncLifetime
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    private static Sentence CreateSentence(string name)
    {
        return new Sentence { SentenceName = name };
    }

    [Theory]
    [InlineData()]
    [InlineData("Viação", "Serviço")]
    public async Task List_ReturnsExpectedItems(params string[] namesInput)
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Sentences.AddRange(namesInput.Select(CreateSentence));
        await dbContext.SaveChangesAsync();

        // Act
        IDocument doc = await _client.GetDocumentAsync("/Sentence/List");

        // Assert
        Assert.Equal(namesInput.Length, await dbContext.Sentences.CountAsync());

        var rows = doc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (Sentence Sentence in dbContext.Sentences)
        {
            Assert.Contains(Sentence.SentenceName, namesInput);

            IElement? row = doc.QuerySelector($"table>tbody>tr[data-id='{Sentence.SentenceId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector($"td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(Sentence.SentenceName, cell.TextContent.Trim());
        }
    }

    [Theory]
    [InlineData()]
    [InlineData("Viação")]
    [InlineData("Viação", "Serviço")]
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
                ["SentenceName"] = name
            };

            await _client.PostAsync("/Sentence/Create", new FormUrlEncodedContent(formData));
        }

        // Assert
        DbSet<Sentence> dbItems = dbContext.Sentences;

        Assert.Equal(namesInput.Length, dbItems.Count());

        IDocument listDoc = await _client.GetDocumentAsync("/Sentence/List");
        var rows = listDoc.QuerySelectorAll("table tbody tr").ToList();
        Assert.Equal(namesInput.Length, rows.Count);

        foreach (Sentence at in dbItems)
        {
            Assert.Contains(at.SentenceName, namesInput);

            IElement? row = listDoc
                .QuerySelector($"table > tbody > tr[data-id='{at.SentenceId}']");
            Assert.NotNull(row);

            IElement? cell = row.QuerySelector("td[data-property='name']");
            Assert.NotNull(cell);
            Assert.Equal(at.SentenceName, cell.TextContent.Trim());
        }
    }


    [Fact]
    public async Task Edit_Get_WhenModelExists_ShowsFormAndFields()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Sentence Sentence = CreateSentence("Viação");

        dbContext.Sentences.Add(Sentence);

        await dbContext.SaveChangesAsync();

        var id = Sentence.SentenceId;

        // Act
        IDocument doc = await _client.GetDocumentAsync($"/Sentence/Edit/{id}");

        // Assert
        Assert.Single(dbContext.Sentences);
        IElement? form = doc.QuerySelector("form[action^='/Sentence/Edit']");
        Assert.NotNull(form);

        IElement idInput = form.QuerySelector("input[name=SentenceId]")!;
        Assert.Equal(id.ToString(), idInput.GetAttribute("value"));

        IElement nameInput = form.QuerySelector("input[name=SentenceName]")!;
        Assert.Equal("Viação", nameInput.GetAttribute("value"));
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Sentence Sentence = CreateSentence("Viação");

        dbContext.Sentences.Add(Sentence);

        await dbContext.SaveChangesAsync();

        var id = Sentence.SentenceId;

        IDocument editDoc = await _client.GetDocumentAsync($"/Sentence/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/Sentence/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["SentenceId"] = id.ToString(),
            ["SentenceName"] = "Atualizado",
        };
        var content = new FormUrlEncodedContent(fields);

        //Act
        await _client.PostAsync(action, content);
        IDocument listDoc = await _client.GetDocumentAsync("/Sentence/List");

        //Assert
        Assert.Single(dbContext.Sentences);
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

        var sentenceName = "prisão";
        Sentence sentence = CreateSentence(sentenceName);
        dbContext.Sentences.Add(sentence);
        var id = sentence.SentenceId;
        await dbContext.SaveChangesAsync();

        IDocument editDoc = await _client.GetDocumentAsync($"/Sentence/Edit/{id}");
        IElement form = editDoc.QuerySelector("form[action^='/Sentence/Edit']")!;
        var action = form.GetAttribute("action")!;

        var fields = new Dictionary<string, string?>
        {
            ["SentenceId"] = id.ToString(),
            ["SentenceName"] = string.Empty
        };

        //Act
        await _client.PostAsync(action, new FormUrlEncodedContent(fields));
        IDocument doc = await _client.GetDocumentAsync("/Sentence/List");

        // Assert
        Assert.Single(dbContext.Sentences);

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
        TestAuthContext.Roles = ["DJ-AUTHORIZED"];
        await using AsyncServiceScope scope = _factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Sentence Sentence = CreateSentence("Viação");
        dbContext.Sentences.Add(Sentence);
        await dbContext.SaveChangesAsync();
        var id = Sentence.SentenceId;

        IDocument listDoc = await _client.GetDocumentAsync("/Sentence/List");

        var token = listDoc
            .QuerySelector("#deleteForm input[name=__RequestVerificationToken]")!
            .GetAttribute("value");

        var fields = new Dictionary<string, string?>
        {
            ["SentenceId"] = id.ToString(),
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/Sentence/Delete/{id}", new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/Sentence/List");

        // Assert 
        Assert.Empty(dbContext.Sentences);
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

        Sentence Sentence = CreateSentence("Viação");
        dbContext.Sentences.Add(Sentence);
        await dbContext.SaveChangesAsync();
        var id = Sentence.SentenceId;

        IDocument listDoc = await _client.GetDocumentAsync("/Sentence/List");
        var token = listDoc
            .QuerySelector("#deleteForm input[name=__RequestVerificationToken]")!
            .GetAttribute("value");

        var fields = new Dictionary<string, string?>
        {
            ["SentenceId"] = "-1",
            ["__RequestVerificationToken"] = token
        };

        //Act
        await _client.PostAsync($"/Sentence/Delete/{int.MaxValue}", new FormUrlEncodedContent(fields));
        IDocument afterDoc = await _client.GetDocumentAsync("/Sentence/List");

        // Assert
        Assert.Single(dbContext.Sentences);
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
        dbContext.RemoveRange(dbContext.Sentences);
        await dbContext.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

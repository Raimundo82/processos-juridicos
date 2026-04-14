using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Services.ProcessManagement;

namespace Processos_Juridicos.Tests.UnitTests.Services;

public class ProcessFileSvcTests
{
    private static AppDbContext CreateDbContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateProcessFile_ShouldPersistAndReturnDto()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        var svc = new ProcessFileSvc(db);

        var dto = new ProcessFileDto
        {
            ProcessId = 10,
            ProcessFileName = "folder/test.pdf",
            ProcessFileType = "application/pdf",
            ProcessFileContent = [1, 2, 3]
        };

        // Act
        ProcessFileDto result = await svc.CreateProcessFile(dto);

        // Assert
        db.ProcessFiles.Should().ContainSingle();

        ProcessFile entity = db.ProcessFiles.First();
        entity.ProcessFileName.Should().Be("test.pdf");
        entity.ProcessFileType.Should().Be("application/pdf");
        entity.ProcessFileContent.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });

        result.ProcessFileName.Should().Be("test.pdf");
    }


    [Fact]
    public async Task DeleteProcessFile_WhenExists_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        var svc = new ProcessFileSvc(db);

        var entity = new ProcessFile
        {
            ProcessFileId = 1,
            ProcessId = 10,
            ProcessFileName = "doc.pdf",
            ProcessFileType = "application/pdf",
            ProcessFileContent = [1]
        };

        db.ProcessFiles.Add(entity);
        await db.SaveChangesAsync();

        // Act
        var result = await svc.DeleteProcessFile(1);

        // Assert
        result.Should().BeTrue();
        db.ProcessFiles.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteProcessFile_WhenNotFound_ShouldReturnFalse()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        var svc = new ProcessFileSvc(db);

        // Act
        var result = await svc.DeleteProcessFile(999);

        // Assert
        result.Should().BeFalse();
    }


    [Fact]
    public async Task GetProcessFileById_WhenExists_ShouldReturnDto()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        var svc = new ProcessFileSvc(db);

        var entity = new ProcessFile
        {
            ProcessFileId = 1,
            ProcessId = 10,
            ProcessFileName = "doc.pdf",
            ProcessFileType = "application/pdf",
            ProcessFileContent = [1]
        };

        db.ProcessFiles.Add(entity);
        await db.SaveChangesAsync();

        // Act
        ProcessFileDto result = await svc.GetProcessFileById(1);

        // Assert
        result.ProcessFileName.Should().Be("doc.pdf");
        result.ProcessFileType.Should().Be("application/pdf");
        result.ProcessFileContent.Should().BeEquivalentTo(new byte[] { 1 });
    }

    [Fact]
    public async Task GetProcessFileById_WhenNotFound_ShouldThrow()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        var svc = new ProcessFileSvc(db);

        // Act
        Func<Task> act = async () => await svc.GetProcessFileById(999);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }


    [Fact]
    public async Task GetAllProcessFilesByProcessId_ShouldReturnFiles()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        var svc = new ProcessFileSvc(db);

        db.ProcessFiles.AddRange(
            new ProcessFile { ProcessFileId = 1, ProcessId = 10, ProcessFileName = "a.pdf" },
            new ProcessFile { ProcessFileId = 2, ProcessId = 10, ProcessFileName = "b.pdf" },
            new ProcessFile { ProcessFileId = 3, ProcessId = 20, ProcessFileName = "c.pdf" }
        );

        await db.SaveChangesAsync();

        // Act
        List<ProcessFileDto> result = await svc.GetAllProcessFilesByProcessId(10);

        // Assert
        result.Should().HaveCount(2);
        result.Select(x => x.ProcessFileName).Should().BeEquivalentTo("a.pdf", "b.pdf");
    }

}

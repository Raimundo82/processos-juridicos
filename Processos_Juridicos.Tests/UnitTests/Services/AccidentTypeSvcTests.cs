using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Services.DomainData;

namespace Processos_Juridicos.Tests.UnitTests.Services;

public class AccidentTypeSvcTests
{
    private static AppDbContext CreateDbContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static AccidentTypeSvc CreateService(AppDbContext db)
    {
        return new AccidentTypeSvc(db);
    }

    [Fact]
    public async Task GetAllAccidentTypes_ReturnsMappedDtos()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        db.AccidentTypes.Add(new AccidentType { AccidentTypeName = "Viação" });
        db.AccidentTypes.Add(new AccidentType { AccidentTypeName = "Serviço" });
        await db.SaveChangesAsync();

        AccidentTypeSvc svc = CreateService(db);

        // Act
        IEnumerable<AccidentTypeDto> result = await svc.GetAllAccidentTypes();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, x => x.AccidentTypeName == "Viação");
        Assert.Contains(result, x => x.AccidentTypeName == "Serviço");
    }

    [Fact]
    public async Task GetAllAccidentTypes_ReturnsZeroResults()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        AccidentTypeSvc svc = CreateService(db);

        // Act
        IEnumerable<AccidentTypeDto> result = await svc.GetAllAccidentTypes();

        // Assert
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetAccidentTypeById_WhenExists_ReturnsDto()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        var entity = new AccidentType { AccidentTypeName = "Viação" };
        db.AccidentTypes.Add(entity);
        await db.SaveChangesAsync();

        AccidentTypeSvc svc = CreateService(db);

        // Act
        AccidentTypeDto result = await svc.GetAccidentTypeById(entity.AccidentTypeId);

        // Assert
        Assert.Equal("Viação", result.AccidentTypeName);
    }

    [Fact]
    public async Task GetAccidentTypeById_WhenNotFound_ThrowsException()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        AccidentTypeSvc svc = CreateService(db);

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            svc.GetAccidentTypeById(999));
    }


    [Fact]
    public async Task CreateAccidentType_CreatesEntityAndReturnsDto()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        AccidentTypeSvc svc = CreateService(db);

        var dto = new AccidentTypeDto { AccidentTypeName = "Novo" };

        // Act
        AccidentTypeDto result = await svc.CreateAccidentType(dto);

        // Assert
        Assert.Equal("Novo", result.AccidentTypeName);
        Assert.Single(db.AccidentTypes);
    }


    [Fact]
    public async Task EditAccidentType_WhenExists_UpdatesEntity()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        var entity = new AccidentType { AccidentTypeName = "Antigo" };
        db.AccidentTypes.Add(entity);
        await db.SaveChangesAsync();

        AccidentTypeSvc svc = CreateService(db);

        var dto = new AccidentTypeDto
        {
            AccidentTypeId = entity.AccidentTypeId,
            AccidentTypeName = "Atualizado"
        };

        // Act
        AccidentTypeDto result = await svc.EditAccidentType(dto);

        // Assert
        Assert.Equal("Atualizado", result.AccidentTypeName);
        Assert.Equal("Atualizado", db.AccidentTypes.First().AccidentTypeName);
    }

    [Fact]
    public async Task EditAccidentType_WhenNotFound_ThrowsException()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        AccidentTypeSvc svc = CreateService(db);

        var dto = new AccidentTypeDto
        {
            AccidentTypeId = 999,
            AccidentTypeName = "Teste"
        };

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            svc.EditAccidentType(dto));
    }


    [Fact]
    public async Task DeleteAccidentType_WhenExists_RemovesEntity()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        var entity = new AccidentType { AccidentTypeName = "Viação" };
        db.AccidentTypes.Add(entity);
        await db.SaveChangesAsync();

        AccidentTypeSvc svc = CreateService(db);

        // Act
        var result = await svc.DeleteAccidentType(entity.AccidentTypeId);

        // Assert
        Assert.True(result);
        Assert.Empty(db.AccidentTypes);
    }

    [Fact]
    public async Task DeleteAccidentType_WhenNotFound_ThrowsException()
    {
        // Arrange
        AppDbContext db = CreateDbContext();
        AccidentTypeSvc svc = CreateService(db);

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            svc.DeleteAccidentType(999));
    }

}

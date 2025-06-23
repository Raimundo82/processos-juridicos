using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Tests;

public class CrimeTypeSvcTests
{
    [Fact]
    public async Task GetAllCrimeTypes_ReturnsAllCrimeTypes()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AppDbContext>();

        db.Crime_types.AddRange(
           new CrimeType { CrimeTypeName = "Corrupção" },
           new CrimeType { CrimeTypeName = "Fraude" }
       );
        await db.SaveChangesAsync();

        // Act
        var svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var result = (await svc.GetAllCrimeTypes())
            .Select(c => c.CrimeTypeName)
            .ToList();


        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("Corrupção", result);
        Assert.Contains("Fraude", result);
    }


    [Fact]
    public async Task GetAllCrimeTypes_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AppDbContext>();

        // Act
        var svc = scope.ServiceProvider.GetRequiredService<ICrimeTypeSvc>();
        var result = await svc.GetAllCrimeTypes();

        // Assert
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetCrimeTypeById_ReturnsTheExpectedCrimeType()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AppDbContext>();

        var crimeType = new CrimeType { CrimeTypeName = "Corrupção" };
        db.Crime_types.Add(crimeType);
        await db.SaveChangesAsync();

        var trackedEntity = await db.Crime_types.FindAsync(crimeType.CrimeTypeId);

        // Act
        var svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var result = (await svc.GetCrimeTypeById(trackedEntity!.CrimeTypeId)).CrimeTypeName;


        // Assert
        Assert.Equal("Corrupção", result);
    }


    [Fact]
    public async Task GetCrimeTypeById_NonExistedId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        // Act
        var svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => svc.GetCrimeTypeById(999));
    }


    [Fact]
    public async Task AddCrimeType_AddsAndReturnsNewCrimeType()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AppDbContext>();


        // Act
        var svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var result = await svc.CreateCrimeType(new DTOs.CrimeTypeDto { CrimeTypeName = "Assédio" });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CrimeTypeId > 0, "EF didn't assign the ID automatically");
        Assert.Equal("Assédio", result.CrimeTypeName);
        Assert.Equal(result.CrimeTypeName, (await db.Crime_types.FindAsync(result.CrimeTypeId))?.CrimeTypeName);
    }


    [Fact]
    public async Task AddCrimeType_DuplicateCrimeType_ThrowsDuplicatedCrimeTypeException()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AppDbContext>();
        var crimeType = new CrimeType { CrimeTypeName = "Corrupção" };
        var crimeTypeDto = new CrimeTypeDto { CrimeTypeName = crimeType.CrimeTypeName };
        db.Crime_types.Add(crimeType);
        await db.SaveChangesAsync();

        // Act
        var svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();

        // Assert
        await Assert.ThrowsAsync<DuplicatedCrimeTypeException>(() => svc.CreateCrimeType(crimeTypeDto));
    }


    [Fact]
    public async Task UpdateCrimeType_UpdatesAndReturnsUpdatedCrimeType()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AppDbContext>();

        var crimeType = new CrimeType { CrimeTypeName = "Corrupção" };
        db.Crime_types.Add(crimeType);
        await db.SaveChangesAsync();

        var trackedEntity = await db.Crime_types.FindAsync(crimeType.CrimeTypeId);
        if (trackedEntity != null) db.Entry(trackedEntity).State = EntityState.Detached;

        // Act
        var svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var result = await svc.EditCrimeType(new CrimeTypeDto { CrimeTypeId = trackedEntity!.CrimeTypeId, CrimeTypeName = "Corrupção Ativa" });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Corrupção Ativa", result.CrimeTypeName);
        Assert.Single(await db.Crime_types.ToListAsync());
        Assert.Equal("Corrupção Ativa", (await db.Crime_types.FindAsync(trackedEntity.CrimeTypeId))?.CrimeTypeName);
    }


    [Fact]
    public async Task EditCrimeType_NonExistentCrimeType_ThrowsEntityNotFoundException()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        // Act
        var svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var nonExistentDto = new CrimeTypeDto { CrimeTypeName = "Inexistente" };

        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => svc.EditCrimeType(nonExistentDto));
    }


    [Fact]
    public async Task DeleteCrimeType_RemovesAndReturnsTrue()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AppDbContext>();

        var crimeType = new CrimeType { CrimeTypeName = "Corrupção" };
        db.Crime_types.Add(crimeType);
        await db.SaveChangesAsync();

        var trackedEntity = await db.Crime_types.FindAsync(crimeType.CrimeTypeId);

        // Act
        var svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var result = await svc.DeleteCrimeType(trackedEntity!.CrimeTypeId);

        // Assert
        Assert.True(result);
        Assert.Empty(await db.Crime_types.ToListAsync());
    }


    [Fact]
    public async Task DeleteCrimeType_NonExistentCrimeType_ReturnsFalse()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        // Act
        var svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var result = await svc.DeleteCrimeType(999);

        // Assert
        Assert.False(result);
    }
}
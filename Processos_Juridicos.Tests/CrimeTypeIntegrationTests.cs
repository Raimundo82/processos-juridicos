using System.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using Processos_Juridicos.Controllers;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Tests;

public class CrimeTypeIntegrationTests
{
    [Fact]
    public async Task List_ReturnsViewWithCrimeTypes()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;
        AppDbContext db = scopedServices.GetRequiredService<AppDbContext>();

        db.Crime_types.AddRange(
           new CrimeType { CrimeTypeName = "Corrupção" },
           new CrimeType { CrimeTypeName = "Fraude" }
       );
        _ = await db.SaveChangesAsync();

        ICrimeTypeSvc svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var controller = new CrimeTypeController(svc, new Mock<IToastNotify>().Object);

        // Act
        IActionResult result = await controller.List();

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        IEnumerable<CrimeTypeDto> model =
    Assert.IsType<IEnumerable<CrimeTypeDto>>(
        viewResult.Model,
        exactMatch: false
    );
        var names = model.Select(x => x.CrimeTypeName).ToList();
        Assert.Equal(2, names.Count);
        Assert.Contains("Corrupção", names);
        Assert.Contains("Fraude", names);
    }

    [Fact]
    public async Task List_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;
        AppDbContext db = scopedServices.GetRequiredService<AppDbContext>();

        db.Crime_types.RemoveRange(db.Crime_types);
        _ = await db.SaveChangesAsync();

        ICrimeTypeSvc svc = scope.ServiceProvider.GetRequiredService<ICrimeTypeSvc>();
        var controller = new CrimeTypeController(svc, new Mock<IToastNotify>().Object);

        // Act
        IActionResult result = await controller.List();

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        IEnumerable<CrimeTypeDto> model =
    Assert.IsType<IEnumerable<CrimeTypeDto>>(
        viewResult.Model,
        exactMatch: false
    );

        Assert.Empty(model);
    }

    [Fact]
    public async Task ListOne_WhenModelStateIsValid_ReturnsViewWithModel()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;
        AppDbContext db = scopedServices.GetRequiredService<AppDbContext>();

        var crimeType = new CrimeType { CrimeTypeName = "Corrupção" };
        _ = db.Crime_types.Add(crimeType);
        _ = await db.SaveChangesAsync();

        ICrimeTypeSvc svc = scope.ServiceProvider.GetRequiredService<ICrimeTypeSvc>();
        var controller = new CrimeTypeController(svc, new Mock<IToastNotify>().Object);

        // Act
        IActionResult result = await controller.ListOne(crimeType.CrimeTypeId);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        CrimeTypeDto model = Assert.IsType<CrimeTypeDto>(viewResult.Model);
        Assert.Equal(crimeType.CrimeTypeId, model.CrimeTypeId);
        Assert.Equal(crimeType.CrimeTypeName, model.CrimeTypeName);
    }

    [Fact]
    public async Task ListOne_WhenModelStateIsInvalid_RedirectsToList()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;

        ICrimeTypeSvc svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var controller = new CrimeTypeController(svc, new Mock<IToastNotify>().Object);

        controller.ModelState.AddModelError("CrimeTypeName", "Campo obrigatório");

        // Act
        IActionResult result = await controller.ListOne(1);

        // Assert
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirectResult.ActionName);
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;

        ICrimeTypeSvc svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var controller = new CrimeTypeController(svc, new Mock<IToastNotify>().Object);

        // Act
        IActionResult result = controller.Create();

        // Assert
        _ = Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_WhenModelIsValid_PersistsCrimeTypeAndRedirects()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;
        AppDbContext db = scopedServices.GetRequiredService<AppDbContext>();

        ICrimeTypeSvc svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var toastNotifyMock = new Mock<IToastNotify>();
        var controller = new CrimeTypeController(svc, toastNotifyMock.Object);

        var model = new CrimeTypeDto { CrimeTypeName = "Assédio" };

        // Act
        IActionResult result = await controller.Create(model);

        // Assert
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirectResult.ActionName);

        CrimeType? crime = await db.Crime_types.FirstOrDefaultAsync(c => c.CrimeTypeName == "Assédio");
        Assert.NotNull(crime);
        Assert.Equal(model!.CrimeTypeName, crime!.CrimeTypeName);

        toastNotifyMock.Verify(t => t.Sucesso(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Create_Post_WhenCrimeTypeAlreadyExists_ThrowsDuplicatedCrimeTypeException()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;
        AppDbContext db = scopedServices.GetRequiredService<AppDbContext>();

        var existing = new CrimeType { CrimeTypeName = "Corrupção" };
        _ = db.Crime_types.Add(existing);
        _ = await db.SaveChangesAsync();

        ICrimeTypeSvc svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var controller = new CrimeTypeController(svc, new Mock<IToastNotify>().Object);

        var duplicateModel = new CrimeTypeDto { CrimeTypeName = "Corrupção" };

        // Act & Assert
        _ = await Assert.ThrowsAsync<DuplicatedCrimeTypeException>(() => controller.Create(duplicateModel));
    }

    [Fact]
    public async Task Create_Post_WhenModelIsInvalid_ReturnsSameViewWithModel()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;

        ICrimeTypeSvc svc = scopedServices.GetRequiredService<ICrimeTypeSvc>();
        var toastNotifyMock = new Mock<IToastNotify>();
        var controller = new CrimeTypeController(svc, toastNotifyMock.Object);

        controller.ModelState.AddModelError("CrimeTypeName", "Campo obrigatório");

        var model = new CrimeTypeDto { CrimeTypeName = "" };

        // Act
        IActionResult result = await controller.Create(model);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        CrimeTypeDto returnedModel = Assert.IsType<CrimeTypeDto>(viewResult.Model);
        Assert.Equal(model.CrimeTypeName, returnedModel.CrimeTypeName);

        toastNotifyMock.Verify(t => t.Sucesso(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Edit_Get_WhenModelExists_ReturnsViewWithModel()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        AppDbContext db = services.GetRequiredService<AppDbContext>();

        var crimeType = new CrimeType { CrimeTypeName = "Crime X" };
        _ = db.Crime_types.Add(crimeType);
        _ = await db.SaveChangesAsync();

        ICrimeTypeSvc svc = services.GetRequiredService<ICrimeTypeSvc>();
        var controller = new CrimeTypeController(svc, new Mock<IToastNotify>().Object);

        // Act
        IActionResult result = await controller.Edit(crimeType.CrimeTypeId);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        CrimeTypeDto model = Assert.IsType<CrimeTypeDto>(viewResult.Model);
        Assert.Equal(crimeType.CrimeTypeId, model.CrimeTypeId);
        Assert.Equal("Crime X", model.CrimeTypeName);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsValid_UpdatesAndRedirects()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        AppDbContext db = services.GetRequiredService<AppDbContext>();

        var original = new CrimeType { CrimeTypeName = "Original" };
        _ = db.Crime_types.Add(original);
        _ = await db.SaveChangesAsync();

        ICrimeTypeSvc svc = services.GetRequiredService<ICrimeTypeSvc>();
        var toastNotifyMock = new Mock<IToastNotify>();
        var controller = new CrimeTypeController(svc, toastNotifyMock.Object);

        var updatedModel = new CrimeTypeDto { CrimeTypeId = original.CrimeTypeId, CrimeTypeName = "Atualizado" };

        // Act
        IActionResult result = await controller.Edit(updatedModel);

        // Assert
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirectResult.ActionName);

        CrimeType? crime = await db.Crime_types.FindAsync(original.CrimeTypeId);
        Assert.Equal("Atualizado", crime?.CrimeTypeName);

        toastNotifyMock.Verify(t => t.Sucesso(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_WhenModelIsInvalid_ReturnsViewWithModel()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        ICrimeTypeSvc svc = services.GetRequiredService<ICrimeTypeSvc>();
        var controller = new CrimeTypeController(svc, new Mock<IToastNotify>().Object);
        var model = new CrimeTypeDto { CrimeTypeId = 1, CrimeTypeName = "Crime A" };

        controller.ModelState.AddModelError("CrimeTypeName", "Campo obrigatório");

        // Act
        IActionResult result = await controller.Edit(model);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        CrimeTypeDto returned = Assert.IsType<CrimeTypeDto>(viewResult.Model);
        Assert.Equal(model.CrimeTypeId, returned.CrimeTypeId);
        Assert.Equal(model.CrimeTypeName, returned.CrimeTypeName);
    }

    [Fact]
    public async Task Delete_WhenDeletionIsSuccessful_RedirectsAndCallsDelete()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        AppDbContext db = services.GetRequiredService<AppDbContext>();

        var crimeType = new CrimeType { CrimeTypeName = "Corrupção" };
        _ = db.Crime_types.Add(crimeType);
        _ = await db.SaveChangesAsync();

        ICrimeTypeSvc svc = services.GetRequiredService<ICrimeTypeSvc>();
        var toastNotifyMock = new Mock<IToastNotify>();
        var controller = new CrimeTypeController(svc, toastNotifyMock.Object);


        // Act
        IActionResult result = await controller.Delete(crimeType.CrimeTypeId);

        // Assert
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirectResult.ActionName);

        Assert.Empty(await db.Crime_types.ToListAsync());
        toastNotifyMock.Verify(t => t.Sucesso(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeletionFails_DisplaysErrorNotification()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        ICrimeTypeSvc svc = services.GetRequiredService<ICrimeTypeSvc>();
        var toastNotifyMock = new Mock<IToastNotify>();
        var controller = new CrimeTypeController(svc, toastNotifyMock.Object);

        // Act
        IActionResult result = await controller.Delete(999);

        // Assert
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirectResult.ActionName);
        toastNotifyMock.Verify(t => t.Error(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenModelStateIsInvalid_RedirectsToListWithoutCallingDelete()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>(Guid.NewGuid().ToString());
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        _ = services.GetRequiredService<ICrimeTypeSvc>();
        var svcMock = new Mock<ICrimeTypeSvc>();
        var toastNotifyMock = new Mock<IToastNotify>();
        var controller = new CrimeTypeController(svcMock.Object, toastNotifyMock.Object);

        controller.ModelState.AddModelError("DeletionError", "Erro simulado");

        // Act
        IActionResult result = await controller.Delete(1);

        // Assert
        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirectResult.ActionName);
        svcMock.Verify(s => s.DeleteCrimeType(It.IsAny<int>()), Times.Never);
    }
}

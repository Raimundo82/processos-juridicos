using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using Processos_Juridicos.Controllers;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Services.Interfaces.UIHelpers;

namespace Processos_Juridicos.Tests.UnitTests.Controllers;

public class AccidentTypeControllerTests
{
    private readonly Mock<IAccidentTypeSvc> _svc = new();
    private readonly Mock<IToastNotify> _toast = new();

    private AccidentTypeController CreateController()
    {
        return new AccidentTypeController(_svc.Object, _toast.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task List_ReturnsViewWithItems()
    {
        // Arrange
        AccidentTypeController controller = CreateController();
        var items = new List<AccidentTypeDto>
        {
            new() { AccidentTypeId = 1, AccidentTypeName = "Viação" }
        };

        _svc.Setup(s => s.GetAllAccidentTypes()).ReturnsAsync(items);

        // Act
        IActionResult result = await controller.List();

        // Assert
        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal(items, view.Model);
    }


    [Fact]
    public void Create_Get_ReturnsView()
    {
        AccidentTypeController controller = CreateController();

        IActionResult result = controller.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsAndCallsService()
    {
        AccidentTypeController controller = CreateController();
        var dto = new AccidentTypeDto { AccidentTypeName = "Viação" };

        IActionResult result = await controller.Create(dto);

        _svc.Verify(s => s.CreateAccidentType(dto), Times.Once);

        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirect.ActionName);

        _toast.Verify(t => t.Sucesso(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        AccidentTypeController controller = CreateController();
        controller.ModelState.AddModelError("x", "erro");

        var dto = new AccidentTypeDto();

        IActionResult result = await controller.Create(dto);

        _svc.Verify(s => s.CreateAccidentType(It.IsAny<AccidentTypeDto>()), Times.Never);

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal(dto, view.Model);
    }


    [Fact]
    public async Task Edit_Get_ValidModel_ReturnsViewWithItem()
    {
        AccidentTypeController controller = CreateController();
        var dto = new AccidentTypeDto { AccidentTypeId = 1, AccidentTypeName = "Viação" };

        _svc.Setup(s => s.GetAccidentTypeById(1)).ReturnsAsync(dto);

        IActionResult result = await controller.Edit(1);

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal(dto, view.Model);
    }

    [Fact]
    public async Task Edit_Get_InvalidModelState_RedirectsToList()
    {
        AccidentTypeController controller = CreateController();
        controller.ModelState.AddModelError("x", "erro");

        IActionResult result = await controller.Edit(1);

        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirect.ActionName);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_UpdatesAndRedirects()
    {
        AccidentTypeController controller = CreateController();
        var dto = new AccidentTypeDto { AccidentTypeId = 1, AccidentTypeName = "Atualizado" };

        IActionResult result = await controller.Edit(dto);

        _svc.Verify(s => s.EditAccidentType(dto), Times.Once);

        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirect.ActionName);

        _toast.Verify(t => t.Sucesso(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsView()
    {
        AccidentTypeController controller = CreateController();
        controller.ModelState.AddModelError("x", "erro");

        var dto = new AccidentTypeDto();

        IActionResult result = await controller.Edit(dto);

        _svc.Verify(s => s.EditAccidentType(It.IsAny<AccidentTypeDto>()), Times.Never);

        ViewResult view = Assert.IsType<ViewResult>(result);
        Assert.Equal(dto, view.Model);
    }


    [Fact]
    public async Task Delete_Success_RedirectsAndShowsSuccessToast()
    {
        AccidentTypeController controller = CreateController();

        _svc.Setup(s => s.DeleteAccidentType(1)).ReturnsAsync(true);

        IActionResult result = await controller.Delete(1);

        _svc.Verify(s => s.DeleteAccidentType(1), Times.Once);
        _toast.Verify(t => t.Sucesso(It.IsAny<string>()), Times.Once);

        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirect.ActionName);
    }

    [Fact]
    public async Task Delete_Failure_RedirectsAndShowsErrorToast()
    {
        AccidentTypeController controller = CreateController();

        _svc.Setup(s => s.DeleteAccidentType(1)).ReturnsAsync(false);

        IActionResult result = await controller.Delete(1);

        _svc.Verify(s => s.DeleteAccidentType(1), Times.Once);
        _toast.Verify(t => t.Error(It.IsAny<string>()), Times.Once);

        RedirectToActionResult redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirect.ActionName);
    }
}

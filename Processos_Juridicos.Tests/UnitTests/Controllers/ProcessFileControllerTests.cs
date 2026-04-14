using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Processos_Juridicos.Controllers;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Services.Interfaces.UIHelpers;

namespace Processos_Juridicos.Tests.UnitTests.Controllers;

public class ProcessFileControllerTests
{
    private readonly Mock<IProcessFileSvc> _filesSvc = new();
    private readonly Mock<IToastNotify> _toast = new();

    private ProcessFileController CreateController()
    {
        return new ProcessFileController(_filesSvc.Object, _toast.Object);
    }

    [Fact]
    public async Task DownloadFile_WhenModelStateInvalid_ShouldRedirect()
    {
        // Arrange
        ProcessFileController controller = CreateController();
        controller.ModelState.AddModelError("x", "erro");

        // Act
        IActionResult result = await controller.DownloadFile(1);

        // Assert
        RedirectToActionResult redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("List");
        redirect.ControllerName.Should().Be("Process");
    }

    [Fact]
    public async Task DownloadFile_WhenFileNotFound_ShouldReturnNotFound()
    {
        // Arrange
        ProcessFileController controller = CreateController();

        _filesSvc.Setup(s => s.GetProcessFileById(1))
                 .ReturnsAsync((ProcessFileDto?)null!);

        // Act
        IActionResult result = await controller.DownloadFile(1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DownloadFile_WhenValid_ShouldReturnFile()
    {
        // Arrange
        ProcessFileController controller = CreateController();

        var fileDto = new ProcessFileDto
        {
            ProcessFileName = "doc.pdf",
            ProcessFileType = "application/pdf",
            ProcessFileContent = [1, 2, 3]
        };

        _filesSvc.Setup(s => s.GetProcessFileById(1))
                 .ReturnsAsync(fileDto);

        // Act
        IActionResult result = await controller.DownloadFile(1);

        // Assert
        FileContentResult file = result.Should().BeOfType<FileContentResult>().Subject;

        file.FileDownloadName.Should().Be("doc.pdf");
        file.ContentType.Should().Be("application/pdf");
        file.FileContents.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Fact]
    public async Task DeleteFile_WhenModelStateInvalid_ShouldRedirectToList()
    {
        // Arrange
        ProcessFileController controller = CreateController();
        controller.ModelState.AddModelError("x", "erro");

        // Act
        IActionResult result = await controller.DeleteFile(10, 20);

        // Assert
        RedirectToActionResult redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("List");
        redirect.ControllerName.Should().Be("Process");
    }

    [Fact]
    public async Task DeleteFile_WhenValid_ShouldDeleteAndRedirect()
    {
        // Arrange
        ProcessFileController controller = CreateController();

        // Act
        IActionResult result = await controller.DeleteFile(10, 20);

        // Assert
        _filesSvc.Verify(s => s.DeleteProcessFile(10), Times.Once);
        _toast.Verify(t => t.Sucesso("File successfully deleted."), Times.Once);

        RedirectToActionResult redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Edit");
        redirect.RouteValues!["id"].Should().Be(20);
    }

}

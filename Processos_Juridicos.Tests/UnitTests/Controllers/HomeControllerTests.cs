using FluentAssertions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Controllers;

namespace Processos_Juridicos.Tests.UnitTests.Controllers;

public class HomeControllerTests
{
    [Fact]
    public void Index_ShouldReturnView()
    {
        // Arrange
        var controller = new HomeController();

        // Act
        IActionResult result = controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Index_ShouldReturnIndexView()
    {
        // Arrange
        var controller = new HomeController();

        // Act
        var result = controller.Index() as ViewResult;

        // Assert
        result!.ViewName.Should().BeNull();
    }

    [Fact]
    public void HomeController_ShouldHaveAuthorizeAttribute()
    {
        // Arrange
        Type type = typeof(HomeController);

        // Act
        var attribute = type.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true);

        // Assert
        attribute.Should().NotBeEmpty();
    }
}

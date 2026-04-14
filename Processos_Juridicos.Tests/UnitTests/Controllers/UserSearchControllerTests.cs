using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Processos_Juridicos.Controllers;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.UserData;

namespace Processos_Juridicos.Tests.UnitTests.Controllers;

public class UserSearchControllerTests
{
    private readonly Mock<IUserDataSvc> _userDataSvc = new();

    private UserSearchController CreateController()
    {
        return new UserSearchController(_userDataSvc.Object);
    }

    [Fact]
    public async Task Search_WhenQueryIsEmpty_ShouldReturnEmptyArray()
    {
        // Arrange
        UserSearchController controller = CreateController();

        // Act
        IActionResult result = await controller.Search("");

        // Assert
        OkObjectResult ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(Array.Empty<object>());
    }

    [Fact]
    public async Task Search_WhenQueryIsValid_ShouldReturnResults()
    {
        // Arrange
        UserSearchController controller = CreateController();

        var users = new List<UserDataModel>
        {
            new() { Nii = "123", DisplayName = "João Silva" }
        };

        _userDataSvc.Setup(s => s.SearchUsersAsync("joao"))
            .ReturnsAsync(users);

        // Act
        IActionResult result = await controller.Search(" joao ");

        // Assert
        OkObjectResult ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(users);
    }


    [Fact]
    public async Task Search_WhenServiceReturnsNull_ShouldReturnEmptyArray()
    {
        // Arrange
        UserSearchController controller = CreateController();

        _userDataSvc.Setup(s => s.SearchUsersAsync("joao"))
                    .ReturnsAsync((IReadOnlyList<UserDataModel>?)null);

        // Act
        IActionResult result = await controller.Search("joao");

        // Assert
        OkObjectResult ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(Array.Empty<object>());
    }

    [Fact]
    public async Task Resolve_WhenIdIsEmpty_ShouldReturnFoundFalse()
    {
        // Arrange
        UserSearchController controller = CreateController();

        // Act
        IActionResult result = await controller.Resolve("");

        // Assert
        OkObjectResult ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(new { found = false });
    }

    [Fact]
    public async Task Resolve_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        UserSearchController controller = CreateController();

        var user = new UserDataModel { Nii = "123", DisplayName = "João Silva" };

        _userDataSvc.Setup(s => s.GetUserByNiiAsync("123"))
            .ReturnsAsync(user);

        // Act
        IActionResult result = await controller.Resolve(" 123 ");

        // Assert
        OkObjectResult ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task Resolve_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        UserSearchController controller = CreateController();

        _userDataSvc.Setup(s => s.GetUserByNiiAsync("999"))
            .ReturnsAsync((UserDataModel?)null);

        // Act
        IActionResult result = await controller.Resolve("999");

        // Assert
        NotFoundObjectResult notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.Value.Should().BeEquivalentTo(new { found = false });
    }

}

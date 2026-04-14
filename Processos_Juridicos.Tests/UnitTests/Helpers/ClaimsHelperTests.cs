using System.Security.Claims;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Processos_Juridicos.Middleware;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.UserData;

namespace Processos_Juridicos.Tests.UnitTests.Helpers;

public class ClaimsHelperTests
{
    private readonly Mock<IUserSvc> _userSvc = new();
    private readonly Mock<IUserDataSvc> _userDataSvc = new();

    private ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_userSvc.Object);
        services.AddSingleton(_userDataSvc.Object);
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task AddCustomClaims_ShouldAddPhotoClaim_WhenPhotoExists()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim("preferred_username", "joao"));

        byte[] photoBytes = [1, 2, 3, 4];

        _userDataSvc.Setup(s => s.FetchUserPhoto("joao"))
            .ReturnsAsync(new UserDataModel { UserPhoto = photoBytes });

        _userSvc.Setup(s => s.GetUserRoleNameByNii("joao"))
            .ReturnsAsync((string?)null);

        IServiceProvider provider = BuildProvider();

        // Act
        await ClaimsHelper.AddCustomClaimsAsync(identity, provider);

        // Assert
        Claim? claim = identity.FindFirst("PhotoBase64");
        claim.Should().NotBeNull();
        claim.Value.Should().Be(Convert.ToBase64String(photoBytes));
    }

    [Fact]
    public async Task AddCustomClaims_ShouldAddRoleClaim_WhenRoleExists()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim("preferred_username", "joao"));

        _userDataSvc.Setup(s => s.FetchUserPhoto("joao"))
            .ReturnsAsync(new UserDataModel { UserPhoto = null });

        _userSvc.Setup(s => s.GetUserRoleNameByNii("joao"))
            .ReturnsAsync("Admin");

        IServiceProvider provider = BuildProvider();

        // Act
        await ClaimsHelper.AddCustomClaimsAsync(identity, provider);

        // Assert
        identity.HasClaim(ClaimTypes.Role, "Admin").Should().BeTrue();
    }

    [Fact]
    public async Task AddCustomClaims_ShouldDoNothing_WhenNoUsername()
    {
        // Arrange
        var identity = new ClaimsIdentity();

        IServiceProvider provider = BuildProvider();

        // Act
        await ClaimsHelper.AddCustomClaimsAsync(identity, provider);

        // Assert
        identity.Claims.Should().BeEmpty();
    }


    [Fact]
    public async Task AddCustomClaims_ShouldNotAddPhoto_WhenPhotoIsNull()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim("preferred_username", "joao"));

        _userDataSvc.Setup(s => s.FetchUserPhoto("joao"))
            .ReturnsAsync(new UserDataModel { UserPhoto = null });

        _userSvc.Setup(s => s.GetUserRoleNameByNii("joao"))
            .ReturnsAsync((string?)null);

        IServiceProvider provider = BuildProvider();

        // Act
        await ClaimsHelper.AddCustomClaimsAsync(identity, provider);

        // Assert
        identity.FindFirst("PhotoBase64").Should().BeNull();
    }
}

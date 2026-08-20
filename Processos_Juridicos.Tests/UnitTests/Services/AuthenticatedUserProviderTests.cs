using System.Security.Claims;
using System.Text.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.UserData;
using Processos_Juridicos.Services.UIHelpers;

namespace Processos_Juridicos.Tests.UnitTests.Services;

public class AuthenticatedUserProviderTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<IUserDataSvc> _mockUserDataSvc;
    private readonly Mock<ISession> _mockSession;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly AuthenticatedUserProvider _provider;

    public AuthenticatedUserProviderTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockUserDataSvc = new Mock<IUserDataSvc>();
        _mockSession = new Mock<ISession>();
        _mockHttpContext = new Mock<HttpContext>();

        _mockHttpContext.Setup(x => x.Session).Returns(_mockSession.Object);
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);

        _provider = new AuthenticatedUserProvider(_mockHttpContextAccessor.Object, _mockUserDataSvc.Object);
    }

    [Fact]
    public async Task GetAsync_WhenUserNotAuthenticated_ReturnsEmptyViewModel()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        _mockHttpContext.Setup(x => x.User).Returns(user);

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.IsAuthenticated.Should().BeFalse();
        result.Nii.Should().Be(string.Empty);
    }

    [Fact]
    public async Task GetAsync_WhenAuthenticated_NoCache_FetchesAndCachesUser()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Test User"),
            new("nii", "123456789"),
            new("display_name", "Test Display Name")
        };

        var identity = new ClaimsIdentity(claims, "test-auth");
        var user = new ClaimsPrincipal(identity);
        _mockHttpContext.Setup(x => x.User).Returns(user);

        var photoBytes = new byte[] { 1, 2, 3 };
        _mockUserDataSvc.Setup(x => x.FetchUserPhoto("m123456789"))
            .ReturnsAsync(new UserDataModel { UserPhoto = photoBytes });

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.IsAuthenticated.Should().BeTrue();
        result.Nii.Should().Be("m123456789");
        result.DisplayName.Should().Be("Test Display Name");
        result.Photo.Should().StartWith("data:image/png;base64,");

        _mockUserDataSvc.Verify(x => x.FetchUserPhoto("m123456789"), Times.Once);
        _mockSession.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenAuthenticated_WithCache_ReturnsCachedUser()
    {
        // Arrange
        var claims = new List<Claim> { new("nii", "123") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test-auth"));
        _mockHttpContext.Setup(x => x.User).Returns(user);

        var cachedUser = new AuthenticatedUserViewModel
        {
            IsAuthenticated = true,
            Nii = "m123",
            DisplayName = "Cached User"
        };
        var json = JsonSerializer.Serialize(cachedUser);

        var sessionBytes = System.Text.Encoding.UTF8.GetBytes(json);
        _mockSession.Setup(x => x.TryGetValue(It.IsAny<string>(), out sessionBytes)).Returns(true);

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.DisplayName.Should().Be("Cached User");
        _mockUserDataSvc.Verify(x => x.FetchUserPhoto(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetAsync_WhenNiiIsEmpty_DoesNotFetchPhoto()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity([], "test-auth"));
        _mockHttpContext.Setup(x => x.User).Returns(user);

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.Photo.Should().Be("/images/default-avatar.webp");
        _mockUserDataSvc.Verify(x => x.FetchUserPhoto(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetAsync_WhenUserHasNoPhoto_ReturnsDefaultAvatar()
    {
        var claims = new List<Claim> { new("nii", "123") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test-auth"));
        _mockHttpContext.Setup(x => x.User).Returns(user);

        _mockUserDataSvc.Setup(x => x.FetchUserPhoto("m123"))
            .ReturnsAsync(new UserDataModel { UserPhoto = null });

        AuthenticatedUserViewModel result = await _provider.GetAsync();

        result.Photo.Should().Be("/images/default-avatar.webp");
        _mockUserDataSvc.Verify(x => x.FetchUserPhoto("m123"), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenUserPhotoIsNull_HandlesGracefully()
    {
        var claims = new List<Claim> { new("nii", "123") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test-auth"));
        _mockHttpContext.Setup(x => x.User).Returns(user);

        _mockUserDataSvc.Setup(x => x.FetchUserPhoto("m123"))
            .ReturnsAsync((UserDataModel?)null);

        AuthenticatedUserViewModel result = await _provider.GetAsync();

        result.Photo.Should().Be("/images/default-avatar.webp");
        _mockUserDataSvc.Verify(x => x.FetchUserPhoto("m123"), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenHttpContextIsNull_ReturnsEmptyViewModel()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null!);

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task GetAsync_WhenUserIdentityIsNull_ReturnsEmptyViewModel()
    {
        // Arrange
        var user = new ClaimsPrincipal();
        _mockHttpContext.Setup(x => x.User).Returns(user);

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task GetAsync_WhenSessionDataIsInvalid_IgnoresCacheAndFetchesUser()
    {
        // Arrange
        var claims = new List<Claim> { new("nii", "123") };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test-auth"));
        _mockHttpContext.Setup(x => x.User).Returns(user);

        var invalidBytes = System.Text.Encoding.UTF8.GetBytes("invalid-json");
        _mockSession.Setup(x => x.TryGetValue(It.IsAny<string>(), out invalidBytes))
            .Returns(true);

        _mockUserDataSvc.Setup(x => x.FetchUserPhoto("m123"))
            .ReturnsAsync(new UserDataModel { UserPhoto = null });

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.IsAuthenticated.Should().BeTrue();
        _mockUserDataSvc.Verify(x => x.FetchUserPhoto("m123"), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenClaimsAreMissing_UsesFallbacks()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity([], "test-auth"));
        _mockHttpContext.Setup(x => x.User).Returns(user);

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.Nii.Should().Be(string.Empty);
        result.DisplayName.Should().Be("Utilizador");
    }

    [Fact]
    public async Task GetAsync_WhenNiiClaimMissing_FallsBackToPreferredUsername()
    {
        var claims = new List<Claim>
        {
            new("preferred_username", "m987654321"),
            new("display_name", "Test User")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test-auth"));
        _mockHttpContext.Setup(x => x.User).Returns(user);

        _mockUserDataSvc.Setup(x => x.FetchUserPhoto("m987654321"))
            .ReturnsAsync(new UserDataModel { UserPhoto = null });

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.Nii.Should().Be("m987654321");
        _mockUserDataSvc.Verify(x => x.FetchUserPhoto("m987654321"), Times.Once);
    }


    [Fact]
    public async Task GetAsync_WhenDisplayNameMissing_FallsBackToClaimsTypeName()
    {
        // Arrange
        var claims = new List<Claim>
    {
        new("nii", "123"),
        new(ClaimTypes.Name, "Fallback Name")
    };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test-auth"));
        _mockHttpContext.Setup(x => x.User).Returns(user);

        _mockUserDataSvc.Setup(x => x.FetchUserPhoto("m123"))
            .ReturnsAsync(new UserDataModel { UserPhoto = null });

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.DisplayName.Should().Be("Fallback Name");
    }

    [Fact]
    public async Task GetAsync_WhenAuthenticated_NoCache_CachesCorrectViewModel()
    {
        // Arrange
        var claims = new List<Claim>
    {
        new("nii", "123"),
        new("display_name", "Test User")
    };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test-auth"));
        _mockHttpContext.Setup(x => x.User).Returns(user);

        _mockUserDataSvc.Setup(x => x.FetchUserPhoto("m123"))
            .ReturnsAsync(new UserDataModel { UserPhoto = null });

        byte[]? capturedBytes = null;
        _mockSession
            .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
            .Callback<string, byte[]>((_, bytes) => capturedBytes = bytes);

        // Act
        await _provider.GetAsync();

        // Assert
        capturedBytes.Should().NotBeNull();
        AuthenticatedUserViewModel? deserializedVm = JsonSerializer.Deserialize<AuthenticatedUserViewModel>(capturedBytes!);
        deserializedVm!.Nii.Should().Be("m123");
        deserializedVm.DisplayName.Should().Be("Test User");
        deserializedVm.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public async Task GetAsync_WhenOnlyNiiClaim_PrefixesWithM()
    {
        var claims = new List<Claim>
        {
            new("nii", "123456"),
            new("display_name", "Test User")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test-auth"));
        _mockHttpContext.Setup(x => x.User).Returns(user);

        _mockUserDataSvc.Setup(x => x.FetchUserPhoto("m123456"))
            .ReturnsAsync(new UserDataModel { UserPhoto = null });

        // Act
        AuthenticatedUserViewModel result = await _provider.GetAsync();

        // Assert
        result.Nii.Should().Be("m123456");
        _mockUserDataSvc.Verify(x => x.FetchUserPhoto("m123456"), Times.Once);
    }
}

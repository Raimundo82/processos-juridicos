using System.Security.Claims;

using FluentAssertions;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Processos_Juridicos.Controllers;

namespace Processos_Juridicos.Tests.UnitTests.Controllers;

public class AccountControllerTests
{
    private static AccountController CreateController(bool authenticated)
    {
        var controller = new AccountController();

        ClaimsIdentity identity = authenticated
            ? new ClaimsIdentity([new Claim("name", "test")], "TestAuth")
            : new ClaimsIdentity();

        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };

        return controller;
    }

    [Fact]
    public void SignIn_WhenNotAuthenticated_ShouldChallenge()
    {
        // Arrange
        AccountController controller = CreateController(authenticated: false);

        // Act
        IActionResult result = controller.SignIn();

        // Assert
        ChallengeResult challenge = result.Should().BeOfType<ChallengeResult>().Subject;
        challenge.AuthenticationSchemes.Should().Contain(OpenIdConnectDefaults.AuthenticationScheme);
    }

    [Fact]
    public void SignIn_WhenAuthenticated_ShouldRedirectToHome()
    {
        // Arrange
        AccountController controller = CreateController(authenticated: true);

        // Act
        IActionResult result = controller.SignIn();

        // Assert
        RedirectToActionResult redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        redirect.ControllerName.Should().Be("Home");
    }

    [Fact]
    public async Task SignOut_WhenNotAuthenticated_ShouldChallenge()
    {
        // Arrange
        AccountController controller = CreateController(authenticated: false);

        // Act
        IActionResult result = await controller.SignOutAsync();

        // Assert
        ChallengeResult challenge = result.Should().BeOfType<ChallengeResult>().Subject;
        challenge.AuthenticationSchemes.Should().Contain(OpenIdConnectDefaults.AuthenticationScheme);
    }

    [Fact]
    public async Task SignOut_WhenAuthenticated_ShouldReturnSignOutResult()
    {
        // Arrange
        AccountController controller = CreateController(authenticated: true);

        controller.ControllerContext.HttpContext.RequestServices =
        new ServiceCollection()
            .AddSingleton<IAuthenticationService, FakeAuthService>()
            .BuildServiceProvider();

        // Act
        IActionResult result = await controller.SignOutAsync();

        // Assert
        SignOutResult signOut = result.Should().BeOfType<SignOutResult>().Subject;

        signOut.AuthenticationSchemes.Should().Contain(CookieAuthenticationDefaults.AuthenticationScheme);
        signOut.AuthenticationSchemes.Should().Contain(OpenIdConnectDefaults.AuthenticationScheme);

        signOut.Properties?.RedirectUri.Should().Be("/");
    }

    [Fact]
    public void AccessDenied_ShouldRedirectToError403()
    {
        // Arrange
        var controller = new AccountController();

        // Act
        IActionResult result = controller.AccessDenied();

        // Assert
        RedirectToActionResult redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        redirect.ControllerName.Should().Be("Error");
        redirect.RouteValues!["code"].Should().Be(403);
    }


    private class FakeAuthService : IAuthenticationService
    {
        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        public Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }

        public Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }

        public Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }

        public Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            return Task.CompletedTask;
        }
    }
}

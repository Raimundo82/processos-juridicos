using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Processos_Juridicos.Controllers;

[AllowAnonymous]
[Route("account")]
public class AccountController(ILogger<AccountController> logger) : Controller
{
    private readonly ILogger<AccountController> logger = logger;

    public IActionResult SignIn()
    {
        return !User.Identity!.IsAuthenticated ? Challenge(OpenIdConnectDefaults.AuthenticationScheme) : RedirectToAction("Index", "Home");
    }

    [HttpGet("signin")]
    public async Task<IActionResult> SignOutAsync()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
        }

        var idToken = await HttpContext.GetTokenAsync("id_token");

        AuthenticateResult? authResult = HttpContext.Features.Get<IAuthenticateResultFeature>()
            ?.AuthenticateResult;

        IEnumerable<AuthenticationToken> tokens = authResult!.Properties!.GetTokens();

        var tokenNames = tokens.Select(token => token.Name).ToArray();

        logger.LogInformation("Token Names: {TokenNames}", string.Join(", ", tokenNames));

        return SignOut(
            new AuthenticationProperties
            {
                RedirectUri = "/",
                Items = { { "id_token_hint", idToken } }
            },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
        );
    }

    public IActionResult AccessDenied()
    {
        return RedirectToAction("AccessDenied", "Home");
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Processos_Juridicos.Controllers;

public class AccountController(ILogger<AccountController> logger) : Controller
{
    private readonly ILogger<AccountController> logger = logger;

    [AllowAnonymous]
    public IActionResult SignIn()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
        }

        return RedirectToAction("Index", "Home");
    }
    [AllowAnonymous]
    public async Task<IActionResult> SignOutAsync()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
        }

        var idToken = await HttpContext.GetTokenAsync("id_token");

        var authResult =
            HttpContext.Features.Get<IAuthenticateResultFeature>()
            ?.AuthenticateResult;

        var tokens = authResult!.Properties!.GetTokens();

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

    [AllowAnonymous]
    public IActionResult AccessDenied() => RedirectToAction("AccessDenied", "Home");
}

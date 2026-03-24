using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Processos_Juridicos.Controllers;

[AllowAnonymous]
[Route("account")]
public class AccountController() : Controller
{
    public IActionResult SignIn()
    {
        return !User.Identity!.IsAuthenticated ? Challenge(OpenIdConnectDefaults.AuthenticationScheme) : RedirectToAction("Index", "Home");
    }

    [HttpGet("signout")]
    public async Task<IActionResult> SignOutAsync()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
        }

        var idToken = await HttpContext.GetTokenAsync("id_token");

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

    [HttpGet("AccessDenied")]
    public IActionResult AccessDenied()
    {
        return RedirectToAction("Index", "Error", new { code = 403 });
    }
}

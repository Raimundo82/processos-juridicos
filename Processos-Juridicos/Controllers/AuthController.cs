using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.Auth;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class AuthController(
    ILdapUserSvc ldapUserSvc,
    IUserSvc userSvc,
    IToastNotify toastNotify) : Controller
{
    private readonly ILdapUserSvc _ldapUserSvc = ldapUserSvc;
    private readonly IUserSvc _userSvc = userSvc;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (!_ldapUserSvc.ValidateAccount(username, password))
        {
            _toastNotify.Error(GlobalTextManager.GetString("InvalidLoginMessage"));
            return View();
        }

        var userRole = await _userSvc.GetUserRoleByNii(username);

        HttpContext.Session.SetString("CargoUser", username);
        HttpContext.Session.SetString("CargoRole", userRole);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}

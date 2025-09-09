using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.Auth;
using Processos_Juridicos.Utilities.TextManager;
using Processos_Juridicos.ViewModels;

namespace Processos_Juridicos.Controllers;

public class AccountController(
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!_ldapUserSvc.ValidateAccount(model.Username, model.Password))
        {
            _toastNotify.Error(GlobalTextManager.GetString("InvalidLoginMessage"));
            return View();
        }

        var userRole = await _userSvc.GetUserRoleByNii(model.Username) ?? string.Empty;

        HttpContext.Session.SetString("SessionUser", model.Username);
        HttpContext.Session.SetString("SessionRole", userRole);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}

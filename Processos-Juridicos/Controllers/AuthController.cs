using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.Auth;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class AuthController(ILdapUserSvc ldapUserSvc, IToastNotify toastNotify) : Controller
{
    private readonly ILdapUserSvc _ldapUserSvc = ldapUserSvc;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        if (_ldapUserSvc.ValidateAccount(username, password))
        {
            HttpContext.Session.SetString("CargoUser", username);
            HttpContext.Session.SetString("CargoRole", "ainda por definir");
            return RedirectToAction("Index", "Home");
        }

        _toastNotify.Error(GlobalTextManager.GetString("InvalidLoginMessage"));
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}

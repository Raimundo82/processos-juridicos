using Microsoft.AspNetCore.Mvc;

namespace Processos_Juridicos.Controllers;

public class LoginController : Controller
{
    public IActionResult Login()
    {
        return View();
    }
}

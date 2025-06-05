using Microsoft.AspNetCore.Mvc;

namespace Processos_Juridicos.Controllers
{
    public class ÁuthControllerController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}

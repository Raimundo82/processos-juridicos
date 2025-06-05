using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Data;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IToastNotify _toastNotify;
        private readonly IApisSvc   _apisSvc;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IToastNotify toastNotify, IApisSvc apisSvc)
        {
            _logger = logger;
            _context = context;
            _toastNotify = toastNotify;
            _apisSvc = apisSvc;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
          //  var listUnits = await _apisSvc.geAlltUnits();

          //  var listUnit = new List<Units>();
          //  foreach (var unit in listUnits)
          //  {
          //      var units = new Units 
          //      { 
          //      unit_code = unit.codUnidade,
          //      unit_acronym = unit.sigUnidade,
          //      unit_name = unit.descUnidades,
          //      sector_code = null
          //      };
          //      listUnit.Add(units);
          //  }

          //  _context.Units.AddRange(listUnit);
          //await  _context.SaveChangesAsync();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

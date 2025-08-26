using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Models;

namespace Processos_Juridicos.Controllers;

[Route("Error/{code?}")]
public class ErrorController : Controller
{
    public IActionResult Index(int code)
    {
        if (ModelState.IsValid)
        {
            var partialName = code switch
            {
                404 => "ErrorPartials/_NotFoundError",
                500 => "ErrorPartials/_ServerError",
                403 => "ErrorPartials/_NotAllowedError",
                _ => "ErrorPartials/_Uncategorized"
            };

            var viewModel = new ErrorViewModel
            {
                ErrorCode = code,
                PartialViewName = partialName
            };

            return View(viewModel);
        }
        return View();
    }
}

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
            // Choose a partial view based on the error code.
            string partialName = code switch
            {
                404 => "ErrorPartials/_NotFoundError",  // Handle 404 errors
                500 => "ErrorPartials/_ServerError",    // Handle 500 errors 
                403 => "ErrorPartials/_NotAllowedError", //Handle 403 errors
                _ => "ErrorPartials/_Uncategorized"      // Default partial for other error codes
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

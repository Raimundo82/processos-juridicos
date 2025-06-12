using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.Middleware.ExceptionHandlers;
public class GlobalExceptionHandler() : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        int statusCode = exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            DbUpdateException => StatusCodes.Status500InternalServerError,
            _ => 0
        };

        httpContext.Response.StatusCode = statusCode;

        if (statusCode == StatusCodes.Status404NotFound)
        {
            httpContext.Response.Redirect("/Error/404");
        }
        else if (statusCode == StatusCodes.Status403Forbidden)
        {
            httpContext.Response.Redirect("/Error/403");
        }
        else
        {
            httpContext.Response.Redirect("/Error");
        }

        await Task.CompletedTask;
        return true;
    }
}

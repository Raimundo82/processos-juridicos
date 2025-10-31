using Microsoft.AspNetCore.Mvc;

namespace Processos_Juridicos.Services.Interfaces.Document;

public interface IViewRenderSvc
{
    public Task<string> RenderViewToStringAsync(ControllerContext context, string viewName, object model);
}

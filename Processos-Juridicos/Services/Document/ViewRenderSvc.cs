using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Processos_Juridicos.Services.Interfaces.Document;

namespace Processos_Juridicos.Services.Document;

public class ViewRenderSvc(ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider) : IViewRenderSvc
{
    private readonly ICompositeViewEngine _viewEngine = viewEngine;
    private readonly ITempDataProvider _tempDataProvider = tempDataProvider;

    public async Task<string> RenderViewToStringAsync(ControllerContext context, string viewName, object model)
    {
        ViewEngineResult viewResult = _viewEngine.FindView(context, viewName, false);

        if (viewResult.View == null)
        {
            throw new ArgumentNullException($"{viewName} não foi encontrada.");
        }

        await using var sw = new StringWriter();
        var viewContext = new ViewContext(
            context,
            viewResult.View,
            new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            { Model = model },
            new TempDataDictionary(context.HttpContext, _tempDataProvider),
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}

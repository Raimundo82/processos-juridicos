using Microsoft.AspNetCore.Mvc;

namespace Processos_Juridicos.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}

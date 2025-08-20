using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Models;

namespace Processos_Juridicos.ViewComponents;

public class GenericTableViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(GenericTableModel model)
    {
        return View(model);
    }
}

using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.ViewComponents;

public class HeaderViewComponent(IWindowsUserSvc userSvc) : ViewComponent
{
    private readonly IWindowsUserSvc _userSvc = userSvc;

    public IViewComponentResult Invoke()
    {
        Models.UserDataModel model = _userSvc.GetUserData();
        return View(model);
    }
}

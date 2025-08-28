using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Services.Interfaces.Auth;

namespace Processos_Juridicos.ViewComponents;

public class HeaderViewComponent(ILdapUserSvc userSvc) : ViewComponent
{
    private readonly ILdapUserSvc _userSvc = userSvc;

    public IViewComponentResult Invoke()
    {
        Models.UserDataModel model = _userSvc.GetLoggedUserData();
        return View(model);
    }
}

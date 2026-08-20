using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.UIHelpers;

namespace Processos_Juridicos.ViewComponents;

public class HeaderViewComponent(IAuthenticatedUserProvider userProvider) : ViewComponent
{
    private readonly IAuthenticatedUserProvider _userProvider = userProvider;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        AuthenticatedUserViewModel model = await _userProvider.GetAsync();
        return View(model);
    }
}

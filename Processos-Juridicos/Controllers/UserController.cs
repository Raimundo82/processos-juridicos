using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;
public class UserController(IUserSvc userSvc, IRoleSvc roleSvc, IToastNotify toastNotify) : Controller
{
    private readonly string EntityName = "Permissão de utilizador";

    private readonly IUserSvc _userSvc = userSvc;

    private readonly IRoleSvc _roleSvc = roleSvc;

    private readonly IToastNotify _toastNotify = toastNotify;


    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateRolesForViewBag();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserDto model)
    {
        if (ModelState.IsValid)
        {
            model.IsUserManuallySet = true;
            await _userSvc.CreateUser(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "A", EntityName, "a"));
            return RedirectToAction(nameof(List));
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (ModelState.IsValid)
        {
            await PopulateRolesForViewBag();


            UserDto model = await _userSvc.GetUserByNii(id);

            model.OriginalUserNii = model.UserNii;
            return View(model);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserDto model)
    {
        if (ModelState.IsValid)
        {
            model.IsUserManuallySet = true;
            await _userSvc.UpdateUser(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "A", EntityName, "a"));
            return RedirectToAction(nameof(List));
        }

        await PopulateRolesForViewBag();
        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string? id)
    {
        if (ModelState.IsValid)
        {
            var success = await _userSvc.RemoveUser(id);

            if (!success)
            {
                _toastNotify.Error(string.Format(GlobalTextManager.GetString("DeleteFailureMessage"), "a", EntityName));
            }
            else
            {
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("DeleteSuccessMessage"), "A", EntityName, "a"));
            }
        }

        return RedirectToAction(nameof(List));
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<UserDto> listUsersDto = await _userSvc.GetAllUsers();
        return View(listUsersDto);
    }

    private async Task PopulateRolesForViewBag()
    {
        IEnumerable<RoleDto> roles = await _roleSvc.GetAllUserRoles();
        var listRoles = roles.Select(x => new SelectListItem
        {
            Text = x.RoleName,
            Value = x.RoleId.ToString()
        }).ToList();

        ViewBag.roles = listRoles;
    }
}

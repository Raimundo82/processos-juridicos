using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class MilitarySecurityController(IMilitarySecuritySvc militarySecuritySvc, IToastNotify toastNotify) : Controller
{
    private readonly string EntityName = "Segurança Militar";

    private readonly IMilitarySecuritySvc _militarySecuritySvc = militarySecuritySvc;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<MilitarySecurityDto> listMilitarySecuritysDto = await _militarySecuritySvc.GetAllMilitarySecurities();
        return View(listMilitarySecuritysDto);
    }

    [HttpGet]
    public async Task<IActionResult> ListOne(int? id)
    {
        if (ModelState.IsValid)
        {
            MilitarySecurityDto security = await _militarySecuritySvc.GetMilitarySecurityById(id);
            return View(security);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MilitarySecurityDto model)
    {
        if (ModelState.IsValid)
        {
            await _militarySecuritySvc.CreateMilitarySecurity(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "A", EntityName, "a"));
            return RedirectToAction("List");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (ModelState.IsValid)
        {
            MilitarySecurityDto model = await _militarySecuritySvc.GetMilitarySecurityById(id);
            return View(model);
        }
        return RedirectToAction(nameof(List));

    }

    [HttpPost]
    public async Task<IActionResult> Edit(MilitarySecurityDto model)
    {
        if (ModelState.IsValid)
        {
            await _militarySecuritySvc.EditMilitarySecurity(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "A", EntityName, "a"));
            return RedirectToAction("List");
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (ModelState.IsValid)
        {
            var success = await _militarySecuritySvc.DeleteMilitarySecurity(id);
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
}

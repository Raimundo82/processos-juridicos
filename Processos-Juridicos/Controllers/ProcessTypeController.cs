using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

[Authorize(Policy = "DJ-ADMINISTRATION")]
public class ProcessTypeController(IProcessTypeSvc processTypeSvc, IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Tipo de Processo";

    private readonly IProcessTypeSvc _processTypeSvc = processTypeSvc;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<ProcessTypeDto> listTypesDto = await _processTypeSvc.GetAllProcessTypes();
        return View(listTypesDto);
    }

    [HttpGet]
    public async Task<IActionResult> ListOne(int? id)
    {
        if (ModelState.IsValid)
        {
            ProcessTypeDto type = await _processTypeSvc.GetProcessTypeById(id);
            return View(type);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProcessTypeDto model)
    {
        if (ModelState.IsValid)
        {
            await _processTypeSvc.CreateProcessType(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "O", EntityName, "o"));
            return RedirectToAction(nameof(List));
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (ModelState.IsValid)
        {
            ProcessTypeDto model = await _processTypeSvc.GetProcessTypeById(id);
            return View(model);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProcessTypeDto model)
    {
        if (ModelState.IsValid)
        {
            await _processTypeSvc.EditProcessType(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));
            return RedirectToAction(nameof(List));
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (ModelState.IsValid)
        {
            var success = await _processTypeSvc.DeleteProcessType(id);
            if (!success)
            {
                _toastNotify.Error(string.Format(GlobalTextManager.GetString("DeleteFailureMessage"), "o", EntityName));
            }
            else
            {
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("DeleteSuccessMessage"), "O", EntityName, "o"));
            }
        }

        return RedirectToAction(nameof(List));
    }
}

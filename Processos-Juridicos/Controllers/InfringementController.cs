using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class InfringementController(IInfringementSvc infringementSvc, IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Artigo Violado";

    private readonly IInfringementSvc _infringementSvc = infringementSvc;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<InfringementDto> infringements = await _infringementSvc.GetAllInfringements();
        return View(infringements);
    }

    [HttpGet]
    public async Task<IActionResult> ListOne(int? id)
    {
        if (ModelState.IsValid)
        {
            InfringementDto infringement = await _infringementSvc.GetInfringementById(id);
            return View(infringement);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(InfringementDto model)
    {
        if (ModelState.IsValid)
        {
            await _infringementSvc.CreateInfringement(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "O", EntityName, "o"));
            return RedirectToAction("List");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (ModelState.IsValid)
        {
            InfringementDto model = await _infringementSvc.GetInfringementById(id);
            return View(model);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(InfringementDto model)
    {
        if (ModelState.IsValid)
        {
            await _infringementSvc.EditInfringement(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));
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
            var success = await _infringementSvc.DeleteInfringement(id);
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

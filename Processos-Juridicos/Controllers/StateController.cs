using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class StateController(IStateSvc stateSvc, IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Estado";

    private readonly IStateSvc _stateSvc = stateSvc;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<StateDto> listStatesDto = await _stateSvc.GetAllStates();
        return View(listStatesDto);
    }

    [HttpGet]
    public async Task<IActionResult> ListOne(int? id)
    {
        if (ModelState.IsValid)
        {
            StateDto state = await _stateSvc.GetStateById(id);
            return View(state);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(StateDto model)
    {
        if (ModelState.IsValid)
        {
            await _stateSvc.CreateState(model);
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
            StateDto model = await _stateSvc.GetStateById(id);
            return View(model);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(StateDto model)
    {
        if (ModelState.IsValid)
        {
            await _stateSvc.EditState(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));
            return RedirectToAction(nameof(List));
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        IActionResult result = RedirectToAction(nameof(List));

        if (ModelState.IsValid)
        {
            var success = await _stateSvc.DeleteState(id);

            if (!success)
            {
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("DeleteFailureMessage"), "o", EntityName));

                return result;
            }
            else
            {
                _toastNotify.Error(string.Format(GlobalTextManager.GetString("DeleteSuccessMessage"), "O", EntityName, "o"));
            }
        }

        return result;
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Processos_Juridicos.Attributes;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Services.Interfaces.UIHelpers;
using Processos_Juridicos.Utilities;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

[Authorize]
public class ProcessController(
    IProcessManagementSvc processManagement,
    IProcessViewDataSvc viewDataSvc,
    IFileValidatorSvc fileValidatorSvc,
    IContextSvc contextSvc,
    IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Processo";
    private const string AccidentProcessTypeName = "Acidentes em serviço";

    private readonly IProcessManagementSvc _processManagement = processManagement;
    private readonly IProcessViewDataSvc _viewDataSvc = viewDataSvc;
    private readonly IFileValidatorSvc _fileValidatorSvc = fileValidatorSvc;
    private readonly IContextSvc _contextSvc = contextSvc;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        if (!User.IsInstrutor() && !User.IsComando() && !User.IsDj())
        {
            return View(new ProcessListViewModel
            {
                Title = "Gestão de Processos",
                Processes = [],
                CanInsertProcess = false,
                HasRole = false
            });
        }

        IEnumerable<ProcessDto> processes = await _processManagement.Processes.GetAllProcesses(User);

        var vm = new ProcessListViewModel
        {
            Title = GetProcessPageTitle(),
            Processes = processes,
            CanInsertProcess = User.IsInstrutor() || User.IsDj(),
            HasRole = true
        };

        return View(vm);
    }

    [Authorize(Policy = "PROCESS-VIEW")]
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (!ModelState.IsValid || id == null)
        {
            _toastNotify.Error(string.Format(GlobalTextManager.GetString("DetailFailureMessage"), "O", EntityName, "o"));
            return RedirectToAction(nameof(List));
        }

        ProcessDto process = await _processManagement.Processes.GetProcessById(id);
        process.UploadedFiles = await _processManagement.ProcessFiles.GetAllProcessFilesByProcessId(id);
        return process == null ? RedirectToAction(nameof(List)) : View(process);
    }

    [Authorize(Policy = "PROCESS-MANAGEMENT")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await _viewDataSvc.PopulateForCreateAsync(ViewData);
        return View();
    }

    [Authorize(Policy = "PROCESS-MANAGEMENT")]
    [HttpPost]
    public async Task<IActionResult> Create(ProcessDto model)
    {
        if (!ModelState.IsValid)
        {
            await _viewDataSvc.PopulateForCreateAsync(ViewData);
            return View(model);
        }

        ApplyCommunicatedPJMRules(model);
        SetAuditFields(model, isNew: true, null);

        ProcessDto insertTarget = await _processManagement.Processes.CreateProcess(model);

        if (!await ValidateAndSaveFiles(insertTarget.ProcessId, model.ProcessFiles))
        {
            return View(model);
        }

        _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "O", EntityName, "o"));
        return RedirectToAction(nameof(List));
    }

    [Authorize(Policy = "PROCESS-MANAGEMENT")]
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(List));
        }

        ProcessDto model = await _processManagement.Processes.GetProcessById(id);

        if (!await UserCanEdit(model))
        {
            return Forbid();
        }

        model.UploadedFiles = await _processManagement.ProcessFiles.GetAllProcessFilesByProcessId(id);
        await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);

        if (ViewData["infringements"] is List<SelectListItem> infrList)
        {
            foreach (SelectListItem? item in infrList.Where(i => model.Infringements.Contains(int.Parse(i.Value))))
            {
                item.Selected = true;
            }
        }

        return View(model);
    }

    [Authorize(Policy = "PROCESS-MANAGEMENT")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProcessDto model)
    {
        if (!ModelState.IsValid)
        {
            await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);
            return View(model);
        }

        if (!await _processManagement.Processes.CanChangeStateAsync((int)model.ProcessId!, model.ProcessStateId))
        {
            ModelState.AddModelError(nameof(model.ProcessStateId), GlobalTextManager.GetString("StateTransitionInvalidMessage"));
            await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);
            return View(model);
        }

        if (model.ProcessType?.ProcessTypeName == AccidentProcessTypeName)
        {
            model.ComunicatedToPjm = false;
        }



        model.ProcessState = await _processManagement.ProcessStates.GetStateById(model.ProcessStateId);

        ProcessDto currentProcess = await _processManagement.Processes.GetProcessById(model.ProcessId);
        if (!await UserCanEdit(currentProcess))
        {
            return Forbid();
        }

        if (!ValidateRequiredFieldsForOpenState(model))
        {
            return await ReturnToEditView(model);
        }


        ApplyCommunicatedPJMRules(model);
        SetAuditFields(model, isNew: false, currentProcess);

        await _processManagement.Processes.EditProcess(model);

        if (!await ValidateAndSaveFiles(model.ProcessId, model.ProcessFiles))
        {
            return await ReturnToEditViewWithFiles(model);
        }

        if (model.FilesToRemove?.Any() == true)
        {
            await RemoveFiles(model.FilesToRemove);
        }

        _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));
        return RedirectToAction(nameof(List));
    }


    [HttpGet]
    public async Task<IActionResult> GetFilterValues()
    {
        ProcessFilterValuesDto values = await _processManagement.Processes.GetFilterValuesAsync();
        return Json(values);
    }

    [Authorize(Policy = "DJ-ADMINISTRATION")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (ModelState.IsValid)
        {
            var success = await _processManagement.Processes.DeleteProcess(id);
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

    #region Helpers

    private string GetProcessPageTitle()
    {
        return User switch
        {
            var u when u.IsInstrutor() => "Os meus Processos",
            var u when u.IsComando() => "Processos da Unidade",
            _ => "Todos os Processos"
        };
    }

    private async Task<bool> UserCanEdit(ProcessDto process)
    {
        var allowedForInstructor = User.IsInstrutor() && (process.ProcessState.StateName == "Em Edição"
            || process.ProcessState.StateName == "Em Validação") && process.CreatedByNii == User.Identity?.Name;

        var isUnitcom = await _contextSvc.Units.IsTheUnitsCommander(process.UnitId, User!.Identity!.Name!);
        var allowedForCommander = User.IsComando() && process.ProcessState.StateName == "Aberto" && isUnitcom;

        return allowedForInstructor || allowedForCommander || User.IsDjAdministration();
    }

    private void ApplyCommunicatedPJMRules(ProcessDto model)
    {
        if (model.ProcessType?.ProcessTypeName == AccidentProcessTypeName)
        {
            model.ComunicatedToPjm = false;
        }
    }

    private void SetAuditFields(ProcessDto model, bool isNew, ProcessDto? process)
    {
        var UserNameFallBack = "Utilizador";

        var displayName = User?.FindFirst("display_name")?.Value ?? UserNameFallBack;
        var nii = User?.FindFirst("preferred_username")?.Value ?? UserNameFallBack;

        if (isNew)
        {
            model.CreatedByName = displayName;
            model.CreatedByNii = nii;
            model.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
        }
        else
        {
            if (process != null)
            {
                model.CreatedBy = process!.CreatedBy;
                model.CreatedByName = process!.CreatedByName;
                model.CreatedByNii = process!.CreatedByNii;
            }

            model.ModifiedByName = displayName;
            model.ModifiedByNii = nii;
            model.ModifiedAt = DateOnly.FromDateTime(DateTime.Now);
        }
    }

    private async Task<bool> ValidateAndSaveFiles(int? processId, IFormFile[]? files)
    {
        if (files == null)
        {
            return true;
        }

        foreach (IFormFile file in files)
        {
            if (!await _fileValidatorSvc.ValidateAndSaveFileAsync(processId, file))
            {
                return false;
            }
        }
        return true;
    }

    private async Task RemoveFiles(IEnumerable<int> fileIds)
    {
        foreach (var fileId in fileIds)
        {
            await _processManagement.ProcessFiles.DeleteProcessFile(fileId);
        }
    }

    private bool ValidateRequiredFieldsForOpenState(ProcessDto model)
    {
        if (model.ProcessState.StateName != "Aberto")
        {
            return true;
        }

        var missing = model.GetType().GetProperties()
            .Where(p => p.Name != nameof(model.ProcessState))
            .Where(p => !Attribute.IsDefined(p, typeof(ExcludedFromValidationAttribute)))
            .Where(p => p.GetValue(model) == null
                     || (p.GetValue(model) is string s && string.IsNullOrWhiteSpace(s)))
            .Select(GetDisplayName)
            .ToList();

        if (missing.Count == 0)
        {
            return true;
        }

        var message = "É necessário preencher os seguintes campos para passar o estado para Aberto: "
            + string.Join(", ", missing);

        ModelState.AddModelError(string.Empty, message);
        return false;
    }

    private static string GetDisplayName(PropertyInfo property)
    {
        DisplayAttribute? displayAttr = property.GetCustomAttributes(typeof(DisplayAttribute), false)
            .Cast<DisplayAttribute>()
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(displayAttr?.Name))
        {
            return displayAttr.Name;
        }

        DisplayNameAttribute? displayNameAttr = property.GetCustomAttributes(typeof(DisplayNameAttribute), false)
            .Cast<DisplayNameAttribute>()
            .FirstOrDefault();

        return !string.IsNullOrWhiteSpace(displayNameAttr?.DisplayName)
            ? displayNameAttr.DisplayName
            : property.Name;
    }

    private async Task<IActionResult> ReturnToEditView(ProcessDto model)
    {
        await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);
        return View(model);
    }

    private async Task<IActionResult> ReturnToEditViewWithFiles(ProcessDto model)
    {
        model = await _processManagement.Processes.GetProcessById(model.ProcessId);
        model.UploadedFiles = await _processManagement.ProcessFiles.GetAllProcessFilesByProcessId(model.ProcessId);
        await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);
        return View(model);
    }
}

#endregion Helpers

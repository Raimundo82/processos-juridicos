using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Services.Interfaces.Ldap;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Utilities;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class ProcessController(
    IProcessManagementSvc processManagement,
    IProcessViewDataSvc viewDataSvc,
    IFileValidatorSvc fileValidatorSvc,
    ILdapUserSvc ldapUserSvc,
    ILegalReferenceSvc legalSvc,
    IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Processo";

    private readonly IProcessManagementSvc _processManagement = processManagement;
    private readonly IProcessViewDataSvc _viewDataSvc = viewDataSvc;
    private readonly IFileValidatorSvc _fileValidatorSvc = fileValidatorSvc;
    private readonly ILdapUserSvc _ldapUserSvc = ldapUserSvc;
    private readonly ILegalReferenceSvc _legalSvc = legalSvc;

    private readonly IToastNotify _toastNotify = toastNotify;

    [Authorize(Policy = "PROCESS-VIEW")]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        if (!User.IsInstrutor() && !User.IsComando() && !User.IsDj())
        {
            return View(new ProcessListViewModel
            {
                Title = "Sem Permissões",
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
    public async Task<IActionResult> Create(ProcessDto model, int?[] selectedInfringements)
    {
        if (ModelState.IsValid)
        {
            model.CreatedBy = _ldapUserSvc.GetLoggedUserData().DisplayName;

            ProcessDto insertTarget = await _processManagement.Processes.CreateProcess(model, selectedInfringements);

            if (model.ProcessFiles != null && model.ProcessFiles.Length > 0)
            {
                foreach (IFormFile? file in model.ProcessFiles)
                {
                    if (!await _fileValidatorSvc.ValidateAndSaveFileAsync(insertTarget.ProcessId, file))
                    {
                        return View(model);
                    }
                }
            }

            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "O", EntityName, "o"));
            return RedirectToAction(nameof(List));
        }

        await _viewDataSvc.PopulateForCreateAsync(ViewData);
        return View(model);
    }

    [Authorize(Policy = "PROCESS-MANAGEMENT")]
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (ModelState.IsValid)
        {
            ProcessDto model = await _processManagement.Processes.GetProcessById(id);
            List<ProcessFileDto> uploadedFiles = await _processManagement.ProcessFiles.GetAllProcessFilesByProcessId(id);
            List<InfringementDto> infringements = await _legalSvc.Infringements.GetAllInfringementsByProcessId(id);
            model.UploadedFiles = uploadedFiles;
            model.Infringements = infringements;

            await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);
            return View(model);
        }

        return RedirectToAction(nameof(List));
    }

    [Authorize(Policy = "PROCESS-MANAGEMENT")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProcessDto model, int?[] selectedInfringements)
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

        ProcessStateDto states = await _processManagement.ProcessStates.GetStateById(model.ProcessStateId);
        model.ProcessState = states;

        await _processManagement.Processes.EditProcess(model, selectedInfringements);

        List<ProcessFileDto> uploadedFiles = await _processManagement.ProcessFiles.GetAllProcessFilesByProcessId(model.ProcessId);

        if (model.ProcessFiles?.Length > 0)
        {
            foreach (IFormFile file in model.ProcessFiles)
            {

                if (!await _fileValidatorSvc.ValidateAndSaveFileAsync(model.ProcessId, file))
                {
                    model = await _processManagement.Processes.GetProcessById(model.ProcessId);
                    model.UploadedFiles = uploadedFiles;
                    await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);
                    return View(model);
                }
            }
        }

        if (model.FilesToRemove?.Count > 0)
        {
            foreach (var fileId in model.FilesToRemove)
            {
                await _processManagement.ProcessFiles.DeleteProcessFile(fileId);
            }
        }

        _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));
        await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);
        model = await _processManagement.Processes.GetProcessById(model.ProcessId);
        uploadedFiles = await _processManagement.ProcessFiles.GetAllProcessFilesByProcessId(model.ProcessId);
        model.UploadedFiles = uploadedFiles;

        return RedirectToAction("Edit", new { id = model.ProcessId });
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

    private string GetProcessPageTitle()
    {
        return User switch
        {
            var u when u.IsInstrutor() => "Os meus Processos",
            var u when u.IsComando() => "Processos da Unidade",
            _ => "Todos os Processos"
        };
    }
}

using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.Auth;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class ProcessController(
    IProcessManagementSvc processManagement,
    IProcessViewDataSvc viewDataSvc,
    IFileValidatorSvc fileValidatorSvc,
    ILdapUserSvc ldapUserSvc,
    IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Processo";

    private readonly IProcessManagementSvc _processManagement = processManagement;
    private readonly IProcessViewDataSvc _viewDataSvc = viewDataSvc;
    private readonly IFileValidatorSvc _fileValidatorSvc = fileValidatorSvc;
    private readonly ILdapUserSvc _ldapUserSvc = ldapUserSvc;

    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<ProcessDto> processes = await _processManagement.Processes.GetAllProcesses();
        return View(processes);
    }

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

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await _viewDataSvc.PopulateForCreateAsync(ViewData);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProcessDto model)
    {
        if (ModelState.IsValid)
        {
            model.CreatedBy = _ldapUserSvc.GetLoggedUserData().DisplayName;

            ProcessDto insertTarget = await _processManagement.Processes.CreateProcess(model);

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

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (ModelState.IsValid)
        {
            ProcessDto model = await _processManagement.Processes.GetProcessById(id);
            List<ProcessFileDto> uploadedFiles = await _processManagement.ProcessFiles.GetAllProcessFilesByProcessId(id);
            model.UploadedFiles = uploadedFiles;


            await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);
            return View(model);
        }

        return RedirectToAction(nameof(List));
    }

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

        await _processManagement.Processes.EditProcess(model);

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
}

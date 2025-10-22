using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Utilities;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class ProcessController(
    IProcessManagementSvc processManagement,
    IProcessViewDataSvc viewDataSvc,
    IFileValidatorSvc fileValidatorSvc,
    IContextSvc contextSvc,
    IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Processo";
    private const string AccidentProcessTypeName = "Acidentes em serviço";
    private const string UserNameFallBack = "Utilizador";
    private readonly IProcessManagementSvc _processManagement = processManagement;
    private readonly IProcessViewDataSvc _viewDataSvc = viewDataSvc;
    private readonly IFileValidatorSvc _fileValidatorSvc = fileValidatorSvc;
    private readonly IContextSvc _contextSvc = contextSvc;

    private readonly IToastNotify _toastNotify = toastNotify;

    [Authorize]
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

        const int defaultPageSize = 25;

        IQueryable<Entities.Process> query = _processManagement.Processes.BuildRestrictedQuery(User);

        List<Entities.Process> firstPageEntities = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip(0)
            .Take(defaultPageSize)
            .ToListAsync();

        IEnumerable<ProcessDto> firstPageDtos = Mapper.MapToToProcessesEnum(firstPageEntities);

        var vm = new ProcessListViewModel
        {
            Title = GetProcessPageTitle(),
            Processes = firstPageDtos,                 // only first page
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

        if (model.ProcessType?.ProcessTypeName == AccidentProcessTypeName)
        {
            model.ComunicatedToPjm = false;
        }


        model.CreatedByName = User?.FindFirst("name")?.Value ?? UserNameFallBack;
        model.CreatedByNii = User?.FindFirst("preferred_username")?.Value ?? UserNameFallBack;

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
            foreach (SelectListItem? item in from SelectListItem item in infrList
                                             where model.Infringements.Contains(int.Parse(item.Value))
                                             select item)
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

        model.ModifiedByName = User?.FindFirst("name")?.Value ?? UserNameFallBack;
        model.ModifiedByNii = User?.FindFirst("preferred_username")?.Value ?? UserNameFallBack;
        model.ModifiedAt = DateOnly.FromDateTime(DateTime.Now);

        ProcessStateDto states = await _processManagement.ProcessStates.GetStateById(model.ProcessStateId);
        model.ProcessState = states;

        ProcessDto currentProcess = await _processManagement.Processes.GetProcessById(model.ProcessId);

        if (!await UserCanEdit(currentProcess))
        {
            return Forbid();
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


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LoadProcesses(int draw, int start, int length, [FromForm(Name = "search[value]")] string? search, string? unitFilter, string? typeFilter, string? stateFilter)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { });
        }

        IQueryable<Entities.Process> query = _processManagement.Processes.BuildRestrictedQuery(User);

        var totalRecords = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Nuipm.Contains(search) ||
                                     p.OficialInstName.Contains(search));
        }

        if (!string.IsNullOrEmpty(unitFilter))
        {
            query = query.Where(p => p.Unit.UnitAcronym == unitFilter);
        }
        if (!string.IsNullOrEmpty(typeFilter))
        {
            query = query.Where(p => p.ProcessType.ProcessTypeName == typeFilter);
        }
        if (!string.IsNullOrEmpty(stateFilter))
        {
            query = query.Where(p => p.ProcessState.StateName == stateFilter);
        }

        var filteredRecords = await query.CountAsync();

        // Materialize first
        List<Entities.Process> page = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip(start)
            .Take(length)
            .ToListAsync();

        var isAdmin = User.IsDjAdministration();

        var data = page.Select(p => new
        {
            processId = p.ProcessId,
            nuipm = p.Nuipm ?? "",
            processTypeName = p.ProcessType?.ProcessTypeName ?? "",
            unitAcronym = p.Unit?.UnitAcronym ?? "",
            oficialInstName = p.OficialInstName ?? "",
            oficialInstTelephone = p.OficialInstTelephone ?? "",
            createdByName = p.CreatedByName ?? "",
            sentenceName = p.Sentence?.SentenceName ?? "",
            createdAt = p.CreatedAt?.ToString("dd-MM-yyyy") ?? "",
            processStateName = p.ProcessState?.StateName ?? "",
            modifiedAt = p.ModifiedAt?.ToString("dd-MM-yyyy") ?? "",
            modifiedByName = p.ModifiedByName ?? "",
            isAdmin
            //actions =
            //    $@" <div class=""d-flex gap-3"">
            //        <a href='/Process/Details/{p.ProcessId}' class='text-primary'>
            //       <i class='bi bi-search'></i>
            //   </a>
            //   <a href='/Process/Edit/{p.ProcessId}' class='text-primary'>
            //       <i class='bi bi-pencil-square'></i>
            //   </a>"
            //    + (isAdmin
            //        ? $@"<a class='text-danger btn-delete'
            //              data-entity='o processo'
            //              data-name='{p.Nuipm}'
            //              data-id='{p.ProcessId}'
            //              data-controller='Process'
            //              data-action='Delete'>
            //              <i class='bi bi-trash-fill'></i>
            //          </a>
            //          </div>"
            //        : "")
        });

        return Json(new
        {
            draw,
            recordsTotal = totalRecords,
            recordsFiltered = filteredRecords,
            data
        });
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

    private async Task<bool> UserCanEdit(ProcessDto process)
    {
        var allowedForInstructor = User.IsInstrutor() && (process.ProcessState.StateName == "Em Edição" || process.ProcessState.StateName == "Em Validação") && process.CreatedByNii == User.Identity?.Name;

        var isUnitcom = await _contextSvc.Units.IsTheUnitsCommander(process.UnitId, User!.Identity!.Name!);
        var allowedForCommander = User.IsComando() && process.ProcessState.StateName == "Aberto" && isUnitcom;

        return allowedForInstructor || allowedForCommander || User.IsDjAdministration();
    }
}



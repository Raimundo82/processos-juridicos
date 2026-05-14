using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Processos_Juridicos.Attributes;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
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

    private readonly Dictionary<int, Expression<Func<Entities.Process, object>>> sortMap = new()
    {
    { 0, p => p.Nuipm },
    { 1, p => p.ProcessType.ProcessTypeName },
    { 2, p => p.Unit.UnitAcronym },
    { 3, p => p.OficialInstName },
    { 4, p => p.OficialInstTelephone },
    { 5, p => p.CreatedByName },
    { 6, p => p.Sentence.SentenceName },
    { 7, p => p.CreatedAt! },
    { 8, p => p.ProcessState.StateName },
    { 9, p => p.ModifiedAt! },
    { 10, p => p.ModifiedByName }
};

    [HttpGet]
    public async Task<IActionResult> List() // default to 10
    {
        var length = 10;

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

        // Fetch only the first `length` processes user can see
        IEnumerable<ProcessDto> allProcesses = await _processManagement.Processes.GetAllProcesses(User);

        IEnumerable<ProcessDto> processes = allProcesses.Take(length);

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
        // 1. ModelState validation
        if (!ModelState.IsValid)
        {
            await _viewDataSvc.PopulateForCreateAsync(ViewData);
            return View(model);
        }

        ApplyInvestigatedUncertainRules(model);
        ApplyCommunicatedPJMRules(model);

        if (model.ProcessFiles != null)
        {
            foreach (IFormFile? file in model.ProcessFiles)
            {
                if (!await _fileValidatorSvc.ValidateFile(file))
                {
                    await _viewDataSvc.PopulateForCreateAsync(ViewData);
                    return View(model);
                }
            }
        }

        if (model.InterestConflictDeclarationUpload == null)
        {
            ModelState.AddModelError(nameof(model.InterestConflictDeclarationUploadFile),
                GlobalTextManager.GetString("UserMustInsertDeclarationConflicts"));

            await _viewDataSvc.PopulateForCreateAsync(ViewData);
            return View(model);
        }

        SetAuditFields(model, isNew: true, null);

        ProcessDto insertTarget = await _processManagement.Processes.CreateProcess(model);

        if (model.ProcessFiles != null)
        {
            foreach (IFormFile? file in model.ProcessFiles)
            {
                await _fileValidatorSvc.SaveFile(insertTarget.ProcessId!.Value, file);
            }
        }

        var declFileId = await _fileValidatorSvc.SaveFile(
        insertTarget.ProcessId!.Value,
        model.InterestConflictDeclarationUpload!
         );

        if (declFileId == null)
        {
            await _viewDataSvc.PopulateForCreateAsync(ViewData);
            return View(model);
        }

        await _processManagement.Processes.SetDeclarationFileAsync(
            insertTarget.ProcessId!.Value,
            declFileId.Value
        );

        // 8. Save declaration file ID to process
        await _processManagement.Processes.SetDeclarationFileAsync(
            insertTarget.ProcessId!.Value,
            (int)declFileId
        );

        // 9. Success toast + redirect
        _toastNotify.Sucesso(string.Format(
            GlobalTextManager.GetString("CreateSuccessMessage"),
            "O", EntityName, "o"
        ));

        return RedirectToAction(nameof(List));
    }



    [Authorize(Policy = "PROCESS-MANAGEMENT")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProcessDto model)
    {
        // Start transaction through ProcessManagement
        await using IDbContextTransaction tx = await _processManagement.BeginTransactionAsync();

        // 1. Validate model
        if (!await ValidateModelAsync(model))
        {
            await tx.RollbackAsync();
            return await ReturnToEditView(model);
        }

        // 2. Load current process
        ProcessDto currentProcess = await _processManagement.Processes.GetProcessById(model.ProcessId);
        if (!await UserCanEdit(currentProcess))
        {
            await tx.RollbackAsync();
            return Forbid();
        }

        // 3. Validate required fields
        if (!ValidateRequiredFieldsForOpenState(model))
        {
            await tx.RollbackAsync();
            return await ReturnToEditView(model);
        }

        // 4. Apply business rules
        ApplyInvestigatedUncertainRules(model);
        ApplyCommunicatedPJMRules(model);
        SetAuditFields(model, isNew: false, currentProcess);

        // 5. Update process
        await _processManagement.Processes.EditProcess(model);

        // 6. Handle declaration file
        if (!await HandleDeclarationFileAsync(model))
        {
            await tx.RollbackAsync();
            return await ReturnToEditViewWithFiles(model);
        }

        // 7. Handle normal files
        if (!await HandleNormalFilesAsync(model))
        {
            await tx.RollbackAsync();
            return await ReturnToEditViewWithFiles(model);
        }

        // 8. Validate declaration removal
        if (!await ValidateDeclarationRemovalAsync(model))
        {
            await tx.RollbackAsync();
            return await ReturnToEditViewWithFiles(model);
        }

        // 9. Remove files if needed
        await RemoveFilesIfNeeded(model);

        // 10. Commit transaction
        await tx.CommitAsync();

        _toastNotify.Sucesso(string.Format(
            GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));

        return RedirectToAction(nameof(List));
    }


    private async Task<bool> ValidateModelAsync(ProcessDto model)
    {
        if (!ModelState.IsValid)
        {
            await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);
            return false;
        }

        if (!await _processManagement.Processes.CanChangeStateAsync(
                (int)model.ProcessId!, model.ProcessStateId))
        {
            ModelState.AddModelError(nameof(model.ProcessStateId),
                GlobalTextManager.GetString("StateTransitionInvalidMessage"));

            await _viewDataSvc.PopulateForEditAsync(ViewData, model.ProcessId);
            return false;
        }

        if (model.ProcessType?.ProcessTypeName == AccidentProcessTypeName)
        {
            model.ComunicatedToPjm = false;
        }

        model.ProcessState = await _processManagement.ProcessStates.GetStateById(model.ProcessStateId);

        return true;
    }

    private async Task<bool> HandleDeclarationFileAsync(ProcessDto model)
    {
        if (model.InterestConflictDeclarationUpload == null)
        {
            return true;
        }

        ProcessFileDto? existing = await _processManagement.ProcessFiles
            .GetDeclarationFileByProcessId(model.ProcessId);

        if (existing != null)
        {
            await _processManagement.ProcessFiles.DeleteProcessFile(existing.ProcessFileId);
        }

        var declFileId = await _fileValidatorSvc.ValidateAndSaveFiles(
            model.ProcessId,
            model.InterestConflictDeclarationUpload
        );

        if (declFileId == null)
        {
            _toastNotify.Error(GlobalTextManager.GetString("FileSaveFailedMessage"));
            return false;
        }

        if (model.ProcessId != null)
        {
            await _processManagement.Processes.SetDeclarationFileAsync(
            model.ProcessId.Value,
            declFileId.Value

        );
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task<bool> HandleNormalFilesAsync(ProcessDto model)
    {
        var normalFiles = model.ProcessFiles?
            .Where(f => f != model.InterestConflictDeclarationUpload)
            .ToList();

        if (normalFiles == null)
        {
            return true;
        }

        foreach (IFormFile? file in normalFiles)
        {
            if (await _fileValidatorSvc.ValidateAndSaveFiles(model.ProcessId, file) == null)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> ValidateDeclarationRemovalAsync(ProcessDto model)
    {
        var currentDeclId = model.InterestConflictDeclarationId
            ?? (await _processManagement.ProcessFiles
                    .GetDeclarationFileByProcessId(model.ProcessId))
                    ?.ProcessFileId;

        // Case 1: No declaration exists AND user didn't upload one
        if (currentDeclId == null && model.InterestConflictDeclarationUpload == null)
        {
            ModelState.AddModelError(nameof(model.InterestConflictDeclarationUploadFile),
                GlobalTextManager.GetString("UserMustInsertDeclarationConflicts"));
            return false;
        }

        // Case 2: User is removing existing declaration AND didn't upload a replacement
        return currentDeclId == null ||
            (model.FilesToRemove?.Contains(currentDeclId.Value)) != true ||
            model.InterestConflictDeclarationUpload != null;
    }

    private async Task RemoveFilesIfNeeded(ProcessDto model)
    {
        if (model.FilesToRemove?.Count > 0)
        {
            await RemoveFiles(model.FilesToRemove);
        }
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
    public async Task<IActionResult> LoadProcesses([FromForm] DataTablesRequest request)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { });
        }

        IQueryable<Entities.Process> query = _processManagement.Processes.BuildRestrictedQuery(User);

        var totalRecords = await query.CountAsync();

        // Filtering
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(p => p.Nuipm.Contains(request.Search) ||
                                     p.OficialInstName.Contains(request.Search));
        }

        if (!string.IsNullOrEmpty(request.UnitFilter))
        {
            query = query.Where(p => p.Unit.UnitAcronym == request.UnitFilter);
        }

        if (!string.IsNullOrEmpty(request.TypeFilter))
        {
            query = query.Where(p => p.ProcessType.ProcessTypeName == request.TypeFilter);
        }

        if (!string.IsNullOrEmpty(request.StateFilter))
        {
            query = query.Where(p => p.ProcessState.StateName == request.StateFilter);
        }

        if (int.TryParse(request.YearFilter, out var yearFilter))
        {
            query = query.Where(p => p.CreatedAt.HasValue && p.CreatedAt.Value.Year == yearFilter);
        }

        var filteredRecords = await query.CountAsync();

        if (sortMap.TryGetValue(request.OrderColumn, out Expression<Func<Entities.Process, object>>? sortExpr))
        {
            query = request.OrderDir == "asc" ? query.OrderBy(sortExpr) : query.OrderByDescending(sortExpr);
        }
        else
        {
            // fallback if no mapping found
            query = query.OrderByDescending(p => p.CreatedAt);
        }

        // Paging
        List<Entities.Process> page = await query
            .Skip(request.Start)
            .Take(request.Length)
            .ToListAsync();

        // Projection
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
            canEdit = UserCanEdit(Mapper.MapToProcessesDto(p)),
            canDelete = UserCanDelete(Mapper.MapToProcessesDto(p))
        });

        return Json(new
        {
            request.Draw,
            recordsTotal = totalRecords,
            recordsFiltered = filteredRecords,
            data
        });
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

    private bool UserCanDelete(ProcessDto process)
    {
        var allowedForDelete = (User.IsDjAdministration() || process.CreatedByNii == User.Identity?.Name)
            && (process.ProcessState.StateName == "Em Edição" || process.ProcessState.StateName == "Em Validação");

        return allowedForDelete;
    }

    private static void ApplyInvestigatedUncertainRules(ProcessDto model)
    {
        if (model.InvestigatedUncertain)
        {
            model.InvestigatedName = null;
            model.InvestigatedGender = "Incerto";
        }
    }

    private static void ApplyCommunicatedPJMRules(ProcessDto model)
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
                model.CreatedBy = process.CreatedBy;
                model.CreatedByName = process.CreatedByName;
                model.CreatedByNii = process.CreatedByNii;
            }

            model.ModifiedByName = displayName;
            model.ModifiedByNii = nii;
            model.ModifiedAt = DateOnly.FromDateTime(DateTime.Now);
        }
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

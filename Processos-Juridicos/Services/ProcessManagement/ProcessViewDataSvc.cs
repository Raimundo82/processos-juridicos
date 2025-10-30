using System.Security.Claims;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;

namespace Processos_Juridicos.Services.ProcessManagement;

public class ProcessViewDataSvc(
    ILegalReferenceSvc legalRefs,
    IContextSvc context,
    IProcessManagementSvc processManagement,
    IHttpContextAccessor httpContextAccessor) : IProcessViewDataSvc
{
    private readonly ILegalReferenceSvc _legalRefs = legalRefs;
    private readonly IContextSvc _context = context;
    private readonly IProcessManagementSvc _processManagement = processManagement;

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private static readonly List<string> Genders = ["Masculino", "Feminino", "Incerto"];
    private const string InitialStateName = "Em Edição";
    private const string SecondStateName = "Em Validação";

    public async Task PopulateForCreateAsync(ViewDataDictionary viewData)
    {
        await PopulateCommonDataAsync(viewData);
        await PopulateStatesForCreateAsync(viewData);
    }

    public async Task PopulateForEditAsync(ViewDataDictionary viewData, int? processId)
    {
        await PopulateCommonDataAsync(viewData);
        await PopulateStatesForEditAsync(viewData, processId);
    }

    private async Task PopulateCommonDataAsync(ViewDataDictionary viewData)
    {
        viewData["genders"] = Genders.Select(
            g => new SelectListItem { Text = g, Value = g }).ToList();

        viewData["units"] = await GetSelectListAsync(
            await _context.Units.GetAllUnits(),
            u => u.UnitName,
            u => u.UnitId ?? 0);

        viewData["compensatingUnits"] = await GetSelectListAsync(
            await _context.Units.GetAllCompensatingUnits(),
            u => u.UnitName,
            u => u.UnitId ?? 0);

        viewData["casualties"] = await GetSelectListAsync(
            await _context.Casualties.GetAllCasualties(),
            c => c.CasualtyName,
            c => c.CasualtyId ?? 0);

        viewData["infringements"] = await GetSelectListAsync(
            await _legalRefs.Infringements.GetAllInfringements(),
            i => i.InfringementName,
            i => i.InfringementId ?? 0);

        viewData["processTypes"] = await _legalRefs.ProcessTypes.GetAllProcessTypes();

        viewData["sentences"] = await GetSelectListAsync(
            await _legalRefs.Sentences.GetAllSentences(),
            s => s.SentenceName,
            s => s.SentenceId ?? 0);

        viewData["accidentTypes"] = await GetSelectListAsync(
            await _legalRefs.AccidentTypes.GetAllAccidentTypes(),
            a => a.AccidentTypeName,
            a => a.AccidentTypeId ?? 0);

        viewData["militarySecurities"] = await GetSelectListAsync(
            await _context.MilitarySecurity.GetAllMilitarySecurities(),
            m => m.MilitarySecurityName,
            m => m.MilitarySecurityId ?? 0);

        viewData["crimeTypes"] = await GetSelectListAsync(
            await _legalRefs.CrimeTypes.GetAllCrimeTypes(),
            c => c.CrimeTypeName,
            c => c.CrimeTypeId ?? 0);
    }

    private async Task PopulateStatesForCreateAsync(ViewDataDictionary viewData)
    {
        var allStates = (await _processManagement.ProcessStates.GetAllStates()).ToList();
        var statesList = new List<SelectListItem>();

        DTOs.ProcessStateDto? initialState = allStates.FirstOrDefault(s => s.StateName == InitialStateName);
        if (initialState != null)
        {
            statesList.Add(new SelectListItem
            {
                Text = initialState.StateName,
                Value = initialState.ProcessStateId.ToString(),
                Selected = true
            });
        }

        if (allStates.Count > 1)
        {
            DTOs.ProcessStateDto? secondState = allStates.FirstOrDefault(s => s.StateName == SecondStateName);
            if (secondState != null)
            {
                statesList.Add(new SelectListItem
                {
                    Text = secondState.StateName,
                    Value = secondState.ProcessStateId.ToString()
                });
            }
        }

        viewData["DisableStateSelection"] = true;
        viewData["states"] = statesList;
    }

    private async Task PopulateStatesForEditAsync(ViewDataDictionary viewData, int? processId)
    {
        DTOs.ProcessDto process = await _processManagement.Processes.GetProcessById(processId);
        var sourceStateId = process.ProcessStateId;

        var userRole = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Role)?.Value;

        IEnumerable<DTOs.ProcessStateDto> states = await _processManagement.ProcessStates.GetAllStates();
        List<DTOs.StateTransitionDto> transitions = await _processManagement.ProcessTransitions.GetAllTransitionsFromSource(sourceStateId, userRole!);

        var allowedTargetIds = transitions.Select(t => t.ToStateId).ToHashSet();

        var listStates = states
            .Where(s => allowedTargetIds.Contains(s.ProcessStateId) || s.ProcessStateId == sourceStateId)
            .Select(s => new SelectListItem
            {
                Text = s.StateName,
                Value = s.ProcessStateId.ToString()
            }).ToList();

        viewData["DisableStateSelection"] = false;
        viewData["states"] = listStates;
    }

    private static Task<List<SelectListItem>> GetSelectListAsync<T>(
        IEnumerable<T> items,
        Func<T, string> textSelector,
        Func<T, int> valueSelector)
    {
        var list = items
            .Select(item => new SelectListItem
            {
                Text = textSelector(item),
                Value = valueSelector(item).ToString()
            })
            .ToList();

        return Task.FromResult(list);
    }
}

using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class ProcessController(IProcessSvc processSvc, IUnitSvc unitSvc, IHarmedOrCasualtySvc casualtiesSvc, IInfringementSvc infringementSvc, IProcessTypeSvc processTypeSvc, ISentenceSvc sentenceSvc, IProcessStateSvc stateSvc, IAccidentTypeSvc accidentTypeSvc, IMilitarySecuritySvc militarySecuritySvc, ICrimeTypeSvc crimeTypeSvc, IProcessFileSvc processFileSvc, IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Processo";

    private readonly ICrimeTypeSvc _crimeTypeSvc = crimeTypeSvc;
    private readonly IMilitarySecuritySvc _militarySecuritySvc = militarySecuritySvc;
    private readonly IProcessStateSvc _stateSvc = stateSvc;
    private readonly IAccidentTypeSvc _accidentTypeSvc = accidentTypeSvc;
    private readonly ISentenceSvc _sentenceSvc = sentenceSvc;
    private readonly IInfringementSvc _infringementSvc = infringementSvc;
    private readonly IProcessTypeSvc _processTypeSvc = processTypeSvc;
    private readonly IUnitSvc _unitSvc = unitSvc;
    private readonly IHarmedOrCasualtySvc _casualtySvc = casualtiesSvc;
    private readonly IProcessSvc _processSvc = processSvc;
    private readonly IToastNotify _toastNotify = toastNotify;
    private readonly IProcessFileSvc _processFileSvc = processFileSvc;

    private readonly string[] permittedFileExtensions = [".pdf", ".jpeg", ".png"];

    private readonly int fileSizeLimit = 5242880; //5MB em base 2

    private static readonly Dictionary<string, List<byte[]>> _fileSignature =
    new()
    {
    { ".pdf", new List<byte[]>
        {
            "%PDF-"u8.ToArray()
        }
    },
    { ".jpeg", new List<byte[]>
        {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
        }
    },
    { ".png", new List<byte[]>
        {
            new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
        }
    },
};

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<ProcessDto> processes = await _processSvc.GetAllProcesses();
        return View(processes);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (ModelState.IsValid)
        {
            ProcessDto process = await _processSvc.GetProcessById(id);
            return View(process);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateViewbags();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProcessDto model)
    {

        if (ModelState.IsValid)
        {
            //TODO: replace this with currently logged in user
            model.CreatedById = "1";

            ProcessDto insertTarget = await _processSvc.CreateProcess(model);

            if (model.ProcessFiles != null && model.ProcessFiles.Length > 0)
            {
                foreach (IFormFile? file in model.ProcessFiles)
                {
                    if (!await ValidateAndSaveFileAsync(insertTarget.ProcessId, file))
                    {
                        return View(model);
                    }
                }
            }

            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "O", EntityName, "o"));
            return RedirectToAction(nameof(List));
        }

        await PopulateViewbags();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (ModelState.IsValid)
        {
            ProcessDto model = await _processSvc.GetProcessById(id);
            List<ProcessFileDto> uploadedFiles = await _processFileSvc.GetAllProcessFilesByProcessId(id);
            model.UploadedFiles = uploadedFiles;


            await PopulateViewbags();
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
            await PopulateViewbags();
            return View(model);
        }

        await _processSvc.EditProcess(model);

        List<ProcessFileDto> uploadedFiles = await _processFileSvc.GetAllProcessFilesByProcessId(model.ProcessId);

        if (model.ProcessFiles?.Length > 0)
        {
            foreach (IFormFile file in model.ProcessFiles)
            {

                if (!await ValidateAndSaveFileAsync(model.ProcessId, file))
                {
                    model = await _processSvc.GetProcessById(model.ProcessId);
                    model.UploadedFiles = uploadedFiles;
                    await PopulateViewbags();
                    return View(model);
                }
            }
        }

        if (model.FilesToRemove?.Count > 0)
        {
            foreach (var fileId in model.FilesToRemove)
            {
                await _processFileSvc.DeleteProcessFile(fileId);
            }
        }

        _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));
        await PopulateViewbags();
        model = await _processSvc.GetProcessById(model.ProcessId);
        uploadedFiles = await _processFileSvc.GetAllProcessFilesByProcessId(model.ProcessId);
        model.UploadedFiles = uploadedFiles;
        return RedirectToAction("Edit", new { id = model.ProcessId });
    }

    private bool ValidateFile(IFormFile file, string ext, MemoryStream ms, out string errorMessage)
    {
        if (!permittedFileExtensions.Contains(ext))
        {
            errorMessage = GlobalTextManager.GetString("FileExtensionNotAllowedMessage");
            return false;
        }

        if (!VerifyFileSignatureCorrect(ms, ext))
        {
            errorMessage = GlobalTextManager.GetString("FileExtensionNotAllowedMessage");
            return false;
        }

        if (file == null || file.Length == 0)
        {
            errorMessage = GlobalTextManager.GetString("EmptyFileMessage");
            return false;
        }

        if (ms.Length > fileSizeLimit)
        {
            errorMessage = GlobalTextManager.GetString("FileSizeTooLargeMessage");
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (ModelState.IsValid)
        {
            var success = await _processSvc.DeleteProcess(id);
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

    private async Task PopulateViewbags()
    {
        PopulateGendersForViewBag();
        await PopulateAccidentTypesForViewBag();
        await PopulateCasualtiesForViewBag();
        await PopulateCrimeTypesForViewBag();
        await PopulateInfringementsForViewBag();
        await PopulateMilitarySecuritiesForViewBag();
        await PopulateProcessTypesForViewBag();
        await PopulateSentencesForViewBag();
        await PopulateStatesForViewBag();
        await PopulateUnitsForViewBag();
    }

    private async Task PopulateUnitsForViewBag()
    {
        IEnumerable<UnitDto> units = await _unitSvc.GetAllUnits();
        var listUnits = units.Select(x => new SelectListItem
        {
            Text = x.UnitName,
            Value = x.UnitId.ToString()
        }).ToList();

        ViewBag.units = listUnits;
    }

    private async Task PopulateCasualtiesForViewBag()
    {
        IEnumerable<HarmedOrCasualtyDto> casualties = await _casualtySvc.GetAllCasualties();
        var listCasualties = casualties.Select(x => new SelectListItem
        {
            Text = x.CasualtyName,
            Value = x.CasualtyId.ToString()
        }).ToList();

        ViewBag.casualties = listCasualties;
    }

    private async Task PopulateInfringementsForViewBag()
    {
        IEnumerable<InfringementDto> infringements = await _infringementSvc.GetAllInfringements();
        var listInfringements = infringements.Select(x => new SelectListItem
        {
            Text = x.InfringementName,
            Value = x.InfringementId.ToString()
        }).ToList();

        ViewBag.infringements = listInfringements;
    }
    private async Task PopulateProcessTypesForViewBag()
    {
        IEnumerable<ProcessTypeDto> processTypes = await _processTypeSvc.GetAllProcessTypes();
        var listProcessTypes = processTypes.Select(x => new SelectListItem
        {
            Text = x.ProcessTypeName,
            Value = x.ProcessTypeId.ToString()
        }).ToList();

        ViewBag.processTypes = listProcessTypes;
    }
    private async Task PopulateSentencesForViewBag()
    {
        IEnumerable<SentenceDto> sentences = await _sentenceSvc.GetAllSentences();
        var listSentences = sentences.Select(x => new SelectListItem
        {
            Text = x.SentenceName,
            Value = x.SentenceId.ToString()
        }).ToList();

        ViewBag.sentences = listSentences;
    }

    private async Task PopulateStatesForViewBag()
    {
        IEnumerable<ProcessStateDto> states = await _stateSvc.GetAllStates();
        var listStates = states.Select(x => new SelectListItem
        {
            Text = x.StateName,
            Value = x.ProcessStateId.ToString()
        }).ToList();

        ViewBag.states = listStates;
    }

    private async Task PopulateAccidentTypesForViewBag()
    {
        IEnumerable<AccidentTypeDto> accidentTypes = await _accidentTypeSvc.GetAllAccidentTypes();
        var listAccidentTypes = accidentTypes.Select(x => new SelectListItem
        {
            Text = x.AccidentTypeName,
            Value = x.AccidentTypeId.ToString()
        }).ToList();

        ViewBag.accidentTypes = listAccidentTypes;
    }

    private async Task PopulateMilitarySecuritiesForViewBag()
    {
        IEnumerable<MilitarySecurityDto> militarySecurities = await _militarySecuritySvc.GetAllMilitarySecurities();
        var listMilitarySecurities = militarySecurities.Select(x => new SelectListItem
        {
            Text = x.MilitarySecurityName,
            Value = x.MilitarySecurityId.ToString()
        }).ToList();

        ViewBag.militarySecurities = listMilitarySecurities;
    }

    private async Task PopulateCrimeTypesForViewBag()
    {
        IEnumerable<CrimeTypeDto> crimeTypes = await _crimeTypeSvc.GetAllCrimeTypes();
        var listCrimeTypes = crimeTypes.Select(x => new SelectListItem
        {
            Text = x.CrimeTypeName,
            Value = x.CrimeTypeId.ToString()
        }).ToList();

        ViewBag.crimeTypes = listCrimeTypes;
    }

    private void PopulateGendersForViewBag()
    {
        var listGenders = new List<string> { "Masculino", "Feminino", "Incerto" };

        var selectGenders = listGenders.Select(item => new SelectListItem
        {
            Text = item,
            Value = item
        }).ToList();


        ViewBag.genders = selectGenders;
    }

    private static bool VerifyFileSignatureCorrect(MemoryStream file, string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return false;
        }

        if (!_fileSignature.TryGetValue(extension, out List<byte[]>? signatures)
            || signatures.Count == 0)
        {
            return false;
        }

        var maxSignatureLength = signatures.Max(sig => sig.Length);

        file.Position = 0;

        var headerBytes = new byte[maxSignatureLength];
        var bytesRead = file.Read(headerBytes, 0, maxSignatureLength);

        return signatures.Any(sig =>
            bytesRead >= sig.Length &&
            headerBytes.AsSpan(0, sig.Length).SequenceEqual(sig));
    }


    private async Task<bool> ValidateAndSaveFileAsync(int? processId, IFormFile file)
    {
        if (processId == null)
        {
            return false;
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        if (!ValidateFile(file, ext, ms, out var error))
        {
            _toastNotify.Error(error);
            return false;
        }

        var trustedName = WebUtility.HtmlEncode(file.FileName);

        // build DTO inline, same as before
        ProcessFileDto fileDto = Mapper.MapToFilesDto(new ProcessFile
        {
            ProcessFileName = file.FileName,
            ProcessFileType = file.ContentType,
            ProcessFileContent = ms.ToArray(),
            ProcessFileTrustedName = trustedName,
            ProcessId = processId.Value
        });

        await _processFileSvc.CreateProcessFile(fileDto);
        return true;
    }

}

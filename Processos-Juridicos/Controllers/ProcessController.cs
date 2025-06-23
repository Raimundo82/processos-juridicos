using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;
using System.Drawing.Text;

namespace Processos_Juridicos.Controllers
{
    public class ProcessController(IProcessSvc processSvc, IUnitSvc unitSvc, IHarmedOrCasualtySvc casualtiesSvc, IInfringementSvc infringementSvc, IProcessTypeSvc processTypeSvc, ISentenceSvc sentenceSvc, IStateSvc stateSvc, IAccidentTypeSvc accidentTypeSvc, IMilitarySecuritySvc militarySecuritySvc, ICrimeTypeSvc crimeTypeSvc, IProcessFileSvc processFileSvc, IToastNotify toastNotify) : Controller
    {
        private const string EntityName = "Processo";

        private readonly ICrimeTypeSvc _crimeTypeSvc = crimeTypeSvc;
        private readonly IMilitarySecuritySvc _militarySecuritySvc = militarySecuritySvc;
        private readonly IStateSvc _stateSvc = stateSvc;
        private readonly IAccidentTypeSvc _accidentTypeSvc = accidentTypeSvc;
        private readonly ISentenceSvc _sentenceSvc = sentenceSvc;
        private readonly IInfringementSvc _infringementSvc = infringementSvc;
        private readonly IProcessTypeSvc _processTypeSvc = processTypeSvc;
        private readonly IUnitSvc _unitSvc = unitSvc;
        private readonly IHarmedOrCasualtySvc _casualtySvc = casualtiesSvc;
        private readonly IProcessSvc _processSvc = processSvc;
        private readonly IToastNotify _toastNotify = toastNotify;
        private readonly IProcessFileSvc _processFileSvc = processFileSvc;


        [HttpGet]
        public async Task<IActionResult> List()
        {
            IEnumerable<ProcessDto> processes = await _processSvc.GetAllProcesses();
            return View(processes);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
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
                model.CreatedById = 1;
                await _processSvc.CreateProcess(model);

                if (model.ProcessFiles != null && model.ProcessFiles.Length > 0)
                {
                    foreach (var file in model.ProcessFiles)
                    {
                        if (file != null && file.Length > 0)
                        {
                            using MemoryStream ms = new();
                            await file.CopyToAsync(ms);

                            ProcessFile fileRecord = new()
                            {
                                ProcessFileName = file.FileName,
                                ProcessFileType = file.ContentType,
                                ProcessFileContent = ms.ToArray(),
                                ProcessId = model.ProcessId
                            };

                            await _processFileSvc.CreateProcessFile(Mapper.MapToFilesDto(fileRecord));
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
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                ProcessDto model = await _processSvc.GetProcessById(id);
                var uploadedFiles = await _processFileSvc.GetAllProcessFilesByProcessId(id);
                model.UploadedFiles = uploadedFiles;


                await PopulateViewbags();
                return View(model);
            }

            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProcessDto model, string? deleteFileId)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewbags();
                return View(model);
            }

            if (!string.IsNullOrEmpty(deleteFileId))
            {
                await TryDeleteFile(deleteFileId, model);
            }

            if (ModelState.IsValid)
            {

                await _processSvc.EditProcess(model);

                if (model.ProcessFiles != null && model.ProcessFiles.Length > 0)
                {

                    foreach (var file in model.ProcessFiles)
                    {
                        if (file != null && file.Length > 0)
                        {
                            using MemoryStream ms = new();
                            await file.CopyToAsync(ms);
                            ProcessFile fileRecord = new()
                            {
                                ProcessFileName = file.FileName,
                                ProcessFileType = file.ContentType,
                                ProcessFileContent = ms.ToArray(),
                                ProcessId = model.ProcessId,
                                RowGuid = 1
                            };

                            await _processFileSvc.CreateProcessFile(Mapper.MapToFilesDto(fileRecord));
                        }
                    }
                }
            }
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));
            return RedirectToAction(nameof(List));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
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

        private async Task<RedirectToActionResult> TryDeleteFile(string deleteFileId, ProcessDto model)
        {
            if (int.TryParse(deleteFileId, out int fileId))
            {
                await _processFileSvc.DeleteProcessFile(fileId);
            }
            return RedirectToAction("Edit", new { id = model.ProcessId });
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
            var units = await _unitSvc.GetAllUnits();
            var listUnits = units.Select(x => new SelectListItem
            {
                Text = x.UnitName,
                Value = x.UnitId.ToString()
            }).ToList();

            ViewBag.units = listUnits;
        }

        private async Task PopulateCasualtiesForViewBag()
        {
            var casualties = await _casualtySvc.GetAllCasualties();
            var listCasualties = casualties.Select(x => new SelectListItem
            {
                Text = x.CasualtyName,
                Value = x.CasualtyId.ToString()
            }).ToList();

            ViewBag.casualties = listCasualties;
        }

        private async Task PopulateInfringementsForViewBag()
        {
            var infringements = await _infringementSvc.GetAllInfringements();
            var listInfringements = infringements.Select(x => new SelectListItem
            {
                Text = x.InfringementName,
                Value = x.InfringementId.ToString()
            }).ToList();

            ViewBag.infringements = listInfringements;
        }
        private async Task PopulateProcessTypesForViewBag()
        {
            var processTypes = await _processTypeSvc.GetAllProcessTypes();
            var listProcessTypes = processTypes.Select(x => new SelectListItem
            {
                Text = x.ProcessTypeName,
                Value = x.ProcessTypeId.ToString()
            }).ToList();

            ViewBag.processTypes = listProcessTypes;
        }
        private async Task PopulateSentencesForViewBag()
        {
            var sentences = await _sentenceSvc.GetAllSentences();
            var listSentences = sentences.Select(x => new SelectListItem
            {
                Text = x.SentenceName,
                Value = x.SentenceId.ToString()
            }).ToList();

            ViewBag.sentences = listSentences;
        }

        private async Task PopulateStatesForViewBag()
        {
            var states = await _stateSvc.GetAllStates();
            var listStates = states.Select(x => new SelectListItem
            {
                Text = x.StateName,
                Value = x.StateId.ToString()
            }).ToList();

            ViewBag.states = listStates;
        }

        private async Task PopulateAccidentTypesForViewBag()
        {
            var accidentTypes = await _accidentTypeSvc.GetAllAccidentTypes();
            var listAccidentTypes = accidentTypes.Select(x => new SelectListItem
            {
                Text = x.AccidentTypeName,
                Value = x.AccidentTypeId.ToString()
            }).ToList();

            ViewBag.accidentTypes = listAccidentTypes;
        }

        private async Task PopulateMilitarySecuritiesForViewBag()
        {
            var militarySecurities = await _militarySecuritySvc.GetAllMilitarySecurities();
            var listMilitarySecurities = militarySecurities.Select(x => new SelectListItem
            {
                Text = x.MilitarySecurityName,
                Value = x.MilitarySecurityId.ToString()
            }).ToList();

            ViewBag.militarySecurities = listMilitarySecurities;
        }

        private async Task PopulateCrimeTypesForViewBag()
        {
            var crimeTypes = await _crimeTypeSvc.GetAllCrimeTypes();
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

    }
}

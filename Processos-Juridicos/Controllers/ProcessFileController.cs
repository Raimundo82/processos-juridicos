using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers;

public class ProcessFileController(IProcessFileSvc filesSvc, IToastNotify toastNotify) : Controller
{
    private readonly IProcessFileSvc _filesSvc = filesSvc;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public async Task<IActionResult> DownloadFile(int id)
    {
        if (ModelState.IsValid)
        {
            ProcessFileDto fileRecord = await _filesSvc.GetProcessFileById(id);
            return fileRecord == null ? NotFound() : File(fileRecord.ProcessFileContent, fileRecord.ProcessFileType, fileRecord.ProcessFileName);
        }

        return RedirectToAction("List", "Process");
    }

    // Delete a file by its ID.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFile(int fileId, int processId)
    {
        if (ModelState.IsValid)
        {
            // Call your file service to delete the file.
            _ = await _filesSvc.DeleteProcessFile(fileId);

            // Optionally add a toast notification here.
            _toastNotify.Sucesso("File successfully deleted.");

            // Redirect back to the edit page of the same process.
            return RedirectToAction("Edit", new { id = processId });
        }

        return RedirectToAction("List", "Process");
    }
}

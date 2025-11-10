using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Event;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces.Document;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Services.Interfaces.UIHelpers;
using Processos_Juridicos.Utilities.Documents;

using IOPath = System.IO.Path;

namespace Processos_Juridicos.Controllers;

[Authorize]
public class DocumentsController(
    IProcessSvc processSvc,
    IToastNotify toastNotify,
    IViewRenderSvc viewRenderSvc) : Controller
{
    private readonly IProcessSvc _processSvc = processSvc;
    private readonly IToastNotify _toastNotify = toastNotify;
    private readonly IViewRenderSvc _viewRenderSvc = viewRenderSvc;

    [HttpGet]
    public async Task<IActionResult> GeneratePDF(int id)
    {
        if (!ModelState.IsValid)
        {
            return HandleInvalidProcess();
        }

        ProcessDto process = await _processSvc.GetProcessById(id);
        if (process == null)
        {
            return HandleInvalidProcess();
        }

        process.OficialInstName = FormatDisplay(process.OficialInstName, process.OficialInstNii);
        process.CreatedByName = FormatDisplay(process.CreatedByName, process.CreatedByNii);

        var htmlContent = await _viewRenderSvc.RenderViewToStringAsync(ControllerContext, "PdfTemplate", process);

        using var memoryStream = new MemoryStream();

        var baseUri = new Uri(IOPath.Combine(Directory.GetCurrentDirectory(), "wwwroot")).AbsoluteUri;
        ConverterProperties props = new ConverterProperties().SetBaseUri(baseUri);

        var writer = new PdfWriter(memoryStream);
        var pdfDoc = new PdfDocument(writer);

        var logoDjPath = IOPath.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", "logo-dj.png");

        pdfDoc.AddEventHandler(PdfDocumentEvent.START_PAGE, new PageHeaderHandler(logoDjPath));
        pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, new PageFooterHandler());

        HtmlConverter.ConvertToPdf(htmlContent, pdfDoc, props);

        pdfDoc.Close();

        return File(memoryStream.ToArray(), "application/pdf", $"Processo_{process.Nuipm}.pdf");
    }

    private RedirectToActionResult HandleInvalidProcess()
    {
        _toastNotify.Error("Ocorreu um erro a gerar o pdf");
        return RedirectToAction("List", "Process");
    }

    private static string FormatDisplay(string? name, string? nii)
    {
        if (string.IsNullOrEmpty(name))
        {
            return "N/A";
        }

        return string.IsNullOrEmpty(nii) ? name : $"{name} ({nii})";
    }
}

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Processos_Juridicos.Utilities.Documents;

internal class PageFooterHandler : AbstractPdfDocumentEventHandler
{
    protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
    {
        var docEvent = (PdfDocumentEvent)@event;

        PdfPage page = docEvent.GetPage();
        var num = docEvent.GetDocument().GetPageNumber(page);

        new Canvas(new PdfCanvas(page), page.GetPageSize())
            .ShowTextAligned(new Paragraph($"Página {num}")
                .SetFontSize(7.5f),
                page.GetPageSize().GetWidth() / 2, 20,
                TextAlignment.CENTER);
    }
}

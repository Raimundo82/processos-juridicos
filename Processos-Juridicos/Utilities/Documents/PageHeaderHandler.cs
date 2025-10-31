using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Processos_Juridicos.Utilities.Documents;

internal class PageHeaderHandler(string logoDjPath) : AbstractPdfDocumentEventHandler
{
    private readonly string _logoDjPath = logoDjPath;

    protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
    {
        var docEvent = (PdfDocumentEvent)@event;
        PdfPage page = docEvent.GetPage();
        Rectangle pageSize = page.GetPageSize();

        var pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), docEvent.GetDocument());
        var canvas = new Canvas(pdfCanvas, pageSize);

        float headerHeight = 60;
        var headerCenterY = pageSize.GetTop() - (headerHeight / 2);

        Paragraph titulo = new Paragraph("Direção Jurídica")
            .SetFontSize(12);
        canvas.ShowTextAligned(titulo, pageSize.GetWidth() / 2, headerCenterY,
            TextAlignment.CENTER, VerticalAlignment.MIDDLE);

        Paragraph visto = new Paragraph("Visto:")
            .SetFontSize(10);
        canvas.ShowTextAligned(visto, pageSize.GetWidth() - 60, headerCenterY,
            TextAlignment.RIGHT, VerticalAlignment.MIDDLE);

        Image logoDj = new Image(ImageDataFactory.Create(_logoDjPath))
            .ScaleToFit(40, 40)
            .SetFixedPosition(40, headerCenterY - 20);
        canvas.Add(logoDj);

        canvas.Close();
    }
}

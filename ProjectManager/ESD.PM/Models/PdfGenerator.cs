using iText.Kernel.Pdf;

namespace ESD.PM.Models
{
    public class PdfGenerator
    {
        public void CreatePdfFromTemplate(string templatePath, string outputPath)
        {
            PdfReader pdfReader = new PdfReader(templatePath);
            PdfWriter pdfWriter = new PdfWriter(outputPath);
            PdfDocument pdfDocument = new PdfDocument(pdfReader, pdfWriter);

            pdfDocument.Close();
        }
    }
}

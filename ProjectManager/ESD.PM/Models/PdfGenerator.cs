using iText.Forms.Fields;
using iText.Forms;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

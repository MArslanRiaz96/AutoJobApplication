using AutoJobApplication.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoJobApplication.Data
{
    public class CvService : ICvService
    {
        public byte[] AddSkillsToCv(byte[] fileData, List<string> skills)
        {
            if (fileData == null || skills == null || skills.Count == 0)
                throw new ArgumentNullException("Invalid file data or skills");

            using (var memoryStream = new MemoryStream(fileData))
            {
                // Handle DOCX file
                if (fileData[0] == 'P' && fileData[1] == 'K')
                {
                    using (WordprocessingDocument doc = WordprocessingDocument.Open(memoryStream, true))
                    {
                        var body = doc.MainDocumentPart.Document.Body;

                        // Fully qualify Paragraph and Text to resolve ambiguity
                        body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                            new DocumentFormat.OpenXml.Wordprocessing.Run(
                                new DocumentFormat.OpenXml.Wordprocessing.Text("Additional Skills: " + string.Join(", ", skills))
                            )
                        ));

                        doc.Save();
                    }
                }
                // Handle PDF file
                else
                {
                    PdfReader pdfReader = new PdfReader(fileData);
                    using (var outputStream = new MemoryStream())
                    {
                        using (PdfStamper stamper = new PdfStamper(pdfReader, outputStream))
                        {
                            PdfContentByte cb = stamper.GetOverContent(pdfReader.NumberOfPages);
                            cb.BeginText();
                            cb.SetFontAndSize(BaseFont.CreateFont(), 12);
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "Additional Skills: " + string.Join(", ", skills), 100, 100, 0);
                            cb.EndText();
                        }
                        return outputStream.ToArray();
                    }
                }
                return memoryStream.ToArray();
            }
        }
    }
}

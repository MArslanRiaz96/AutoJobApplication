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

            try
            {
                // Check if the file is a DOCX file
                if (fileData.Length > 4 && fileData[0] == 'P' && fileData[1] == 'K')
                {
                    using (var memoryStream = new MemoryStream(fileData))
                    {
                        using (WordprocessingDocument doc = WordprocessingDocument.Open(memoryStream, true))
                        {
                            var body = doc.MainDocumentPart.Document.Body;

                            foreach (var paragraph in body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                            {
                                if (paragraph.InnerText.Contains("Web Development", StringComparison.OrdinalIgnoreCase))
                                {
                                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(", " + string.Join(", ", skills))));
                                    break;
                                }
                            }

                            doc.Save();
                        }

                        return memoryStream.ToArray();
                    }
                }
                // Check if the file is a PDF file
                else if (fileData.Length > 4 && fileData[0] == '%' && fileData[1] == 'P' && fileData[2] == 'D' && fileData[3] == 'F')
                {
                    using (var outputStream = new MemoryStream())
                    {
                        using (PdfReader pdfReader = new PdfReader(fileData))
                        {
                            using (PdfStamper pdfStamper = new PdfStamper(pdfReader, outputStream))
                            {
                                PdfContentByte cb = pdfStamper.GetOverContent(pdfReader.NumberOfPages);
                                cb.BeginText();
                                cb.SetFontAndSize(BaseFont.CreateFont(), 12);
                                cb.ShowTextAligned(Element.ALIGN_LEFT, "Additional Skills: " + string.Join(", ", skills), 100, 100, 0);
                                cb.EndText();
                            }
                        }

                        return outputStream.ToArray();
                    }
                }
                else
                {
                    throw new FileFormatException("Unsupported file format. Please upload a DOCX or PDF file.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred while processing the file.", ex);
            }
        }
    }
}

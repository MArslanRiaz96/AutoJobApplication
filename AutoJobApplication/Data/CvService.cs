using AutoJobApplication.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using OpenXmlParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

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
                if (IsDocxFile(fileData))
                {
                    return AddSkillsToDocx(fileData, skills);
                }
                else if (IsPdfFile(fileData))
                {
                    return AddSkillsToPdf(fileData, skills);
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

        private bool IsDocxFile(byte[] fileData)
        {
            return fileData.Length > 4 && fileData[0] == 'P' && fileData[1] == 'K';
        }

        private bool IsPdfFile(byte[] fileData)
        {
            return fileData.Length > 4 && fileData[0] == '%' && fileData[1] == 'P' && fileData[2] == 'D' && fileData[3] == 'F';
        }

        private byte[] AddSkillsToDocx(byte[] fileData, List<string> skills)
        {
            using (var memoryStream = new MemoryStream(fileData))
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Open(memoryStream, true))
                {
                    var body = doc.MainDocumentPart.Document.Body;
                    foreach (var para in body.Descendants<OpenXmlParagraph>())
                    {
                        if (para.InnerText.Contains("Web Development"))
                        {
                            var run = para.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run());
                            run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text(", " + string.Join(", ", skills)));
                            break;
                        }
                    }
                    doc.Save();
                }
                return memoryStream.ToArray();
            }
        }

        private byte[] AddSkillsToPdf(byte[] fileData, List<string> skills)
        {
            using (var memoryStream = new MemoryStream(fileData))
            {
                using (PdfReader reader = new PdfReader(memoryStream))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        using (PdfStamper stamper = new PdfStamper(reader, outputStream))
                        {
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                PdfContentByte contentByte = stamper.GetOverContent(i);
                                ColumnText.ShowTextAligned(contentByte, Element.ALIGN_LEFT,
                                    new Phrase(", " + string.Join(", ", skills), FontFactory.GetFont(FontFactory.HELVETICA, 12)),
                                    100, 100, 0);  // Adjust these coordinates as needed
                            }
                        }
                        return outputStream.ToArray();
                    }
                }
            }
        }
    }
}

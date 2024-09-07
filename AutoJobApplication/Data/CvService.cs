using AutoJobApplication.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
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
            // Check if it's a DOCX file by signature
            return fileData.Length > 4 && fileData[0] == 0x50 && fileData[1] == 0x4B; // PK Zip signature
        }

        private bool IsPdfFile(byte[] fileData)
        {
            // Check if it's a PDF file by signature
            return fileData.Length > 4 && fileData[0] == 0x25 && fileData[1] == 0x50 && fileData[2] == 0x44 && fileData[3] == 0x46; // %PDF
        }

        private byte[] AddSkillsToDocx(byte[] fileData, List<string> skills)
        {
            using (var memoryStream = new MemoryStream(fileData))
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Open(memoryStream, true))
                {
                    var body = doc.MainDocumentPart.Document.Body;
                    bool foundWebDev = false;

                    // Search for "Technical Skills" first
                    foreach (var para in body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                    {
                        if (para.InnerText.Contains("Technical Skills") && !foundWebDev)
                        {
                            var nextPara = para.NextSibling<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                            while (nextPara != null && !nextPara.InnerText.Contains("Work Experience"))
                            {
                                if (nextPara.InnerText.Contains("Web Development"))
                                {
                                    foundWebDev = true;
                                    var run = nextPara.Elements<DocumentFormat.OpenXml.Wordprocessing.Run>().LastOrDefault();
                                    if (run != null)
                                    {
                                        run.Append(new Text(", " + string.Join(", ", skills)));  // Append skills directly
                                    }
                                    else
                                    {
                                        nextPara.Append(new Run(new Text(", " + string.Join(", ", skills))));
                                    }
                                    break;
                                }
                                nextPara = nextPara.NextSibling<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                            }
                        }
                        if (foundWebDev) break;
                    }

                    if (!foundWebDev)
                    {
                        throw new InvalidOperationException("The 'Web Development' section was not found in the document.");
                    }

                    doc.Save();
                }
                return memoryStream.ToArray();
            }
        }


        private byte[] AddSkillsToPdf(byte[] fileData, List<string> skills)
        {
            MemoryStream outputStream = new MemoryStream();  // This stream will receive the modified PDF data.
            using (var memoryStream = new MemoryStream(fileData))
            {
                using (PdfReader reader = new PdfReader(memoryStream))
                {
                    using (PdfStamper stamper = new PdfStamper(reader, outputStream))
                    {
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            var strategy = new SimpleTextExtractionStrategy();
                            string pageContent = PdfTextExtractor.GetTextFromPage(reader, i, strategy);

                            if (pageContent.Contains("Web Development"))
                            {
                                PdfContentByte canvas = stamper.GetOverContent(i);
                                canvas.BeginText();
                                canvas.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED), 12);
                                // Adjust the text position appropriately
                                canvas.SetTextMatrix(350, 350);  // Adjust this to match the location where you want to add text
                                canvas.ShowText(", " + string.Join(", ", skills));
                                canvas.EndText();
                            }
                        }
                    }
                }
            }
            outputStream.Flush(); // Ensure all content is written to the stream
            return outputStream.ToArray();  // Return the modified PDF data
        }


    }
}

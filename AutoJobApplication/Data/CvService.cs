using AutoJobApplication.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;

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
            try
            {
                using (var memoryStream = new MemoryStream(fileData))
                {
                    using (WordprocessingDocument doc = WordprocessingDocument.Open(memoryStream, true))
                    {
                        var body = doc.MainDocumentPart.Document.Body;
                        bool skillsAppended = false;

                        foreach (var paragraph in body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                        {
                            // Check if the paragraph contains "Web Development"
                            if (paragraph.InnerText.Contains("Web Development", StringComparison.OrdinalIgnoreCase))
                            {
                                var run = paragraph.Elements<DocumentFormat.OpenXml.Wordprocessing.Run>().FirstOrDefault();

                                if (run != null)
                                {
                                    var textElement = run.GetFirstChild<Text>();
                                    if (textElement != null)
                                    {
                                        // Append new skills to the "Web Development" section
                                        textElement.Text += ", " + string.Join(", ", skills);
                                    }
                                    else
                                    {
                                        run.AppendChild(new Text(", " + string.Join(", ", skills)));
                                    }
                                }
                                else
                                {
                                    // If no Run found, create a new one
                                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(", " + string.Join(", ", skills))));
                                }

                                skillsAppended = true;
                                break;
                            }
                        }

                        if (!skillsAppended)
                        {
                            throw new InvalidOperationException("The 'Web Development' section was not found in the document.");
                        }

                        doc.Save();
                    }

                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding skills to DOCX: " + ex.Message);
                throw new InvalidOperationException("Failed to process the DOCX file.", ex);
            }
        }

        private byte[] AddSkillsToPdf(byte[] fileData, List<string> skills)
        {
            try
            {
                using (var outputStream = new MemoryStream())
                {
                    using (PdfReader pdfReader = new PdfReader(fileData))
                    {
                        using (PdfStamper pdfStamper = new PdfStamper(pdfReader, outputStream))
                        {
                            PdfContentByte cb = pdfStamper.GetOverContent(pdfReader.NumberOfPages);
                            cb.BeginText();
                            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED), 12);

                            // Define the position where the skills should be added
                            float x = 100; // X coordinate
                            float y = 100; // Y coordinate (adjust as needed)

                            cb.ShowTextAligned(Element.ALIGN_LEFT, "Additional Skills: " + string.Join(", ", skills), x, y, 0);
                            cb.EndText();
                        }
                    }

                    return outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding skills to PDF: " + ex.Message);
                throw new InvalidOperationException("Failed to process the PDF file.", ex);
            }
        }
    }
}

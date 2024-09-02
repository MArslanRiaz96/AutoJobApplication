using AutoJobApplication.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
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
                            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                            {
                                PdfContentByte cb = pdfStamper.GetOverContent(i);
                                var textStrategy = new MyLocationTextExtractionStrategy();
                                string extractedText = PdfTextExtractor.GetTextFromPage(pdfReader, i, textStrategy);

                                Console.WriteLine($"Extracted Text from Page {i}: {extractedText}");

                                var location = textStrategy.GetLocation("Web Development");

                                if (location != null)
                                {
                                    float x = location.Value.x;
                                    float y = location.Value.y;

                                    cb.BeginText();
                                    cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED), 12);

                                    string skillsText = ", " + string.Join(", ", skills);
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, skillsText, x + 10, y, 0);

                                    cb.EndText();
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Could not find 'Web Development' on this page.");
                                }
                            }
                        }

                        return outputStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding skills to PDF: " + ex.Message);
                throw new InvalidOperationException("Failed to process the PDF file.", ex);
            }
        }


    }

    public class LocationTextExtractionStrategy
    {
        public List<(float x, float y)> GetCharInfo(string searchText)
        {
            // Implement logic to capture character positions for the given searchText
            // You may need to override RenderText method to capture the coordinates of each character
            // This example is simplified
            return new List<(float x, float y)>();
        }
    }
    public class MyLocationTextExtractionStrategy : ITextExtractionStrategy
    {
        public class TextChunk
        {
            public string Text { get; set; }
            public Vector StartLocation { get; set; }

            public TextChunk(string text, Vector startLocation)
            {
                Text = text;
                StartLocation = startLocation;
            }
        }

        private readonly List<TextChunk> _chunks = new List<TextChunk>();

        public void RenderText(TextRenderInfo renderInfo)
        {
            Vector startLocation = renderInfo.GetBaseline().GetStartPoint();
            string text = renderInfo.GetText();

            _chunks.Add(new TextChunk(text, startLocation));
        }

        public string GetResultantText() => string.Join(" ", _chunks.Select(chunk => chunk.Text));

        public void BeginTextBlock() { }
        public void EndTextBlock() { }
        public void RenderImage(ImageRenderInfo renderInfo) { }

        public (float x, float y)? GetLocation(string searchText)
        {
            string concatenatedText = string.Join("", _chunks.Select(chunk => chunk.Text));

            int index = concatenatedText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                // Find the chunk that contains the start of the searchText
                foreach (var chunk in _chunks)
                {
                    if (concatenatedText.Contains(chunk.Text))
                    {
                        return (chunk.StartLocation[Vector.I1], chunk.StartLocation[Vector.I2]);
                    }
                }
            }

            return null;
        }
    }




}

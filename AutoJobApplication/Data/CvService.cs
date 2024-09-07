using AutoJobApplication.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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
                    return UpdateSkillsInPdf(fileData, skills);
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
            // Implement the logic for DOCX manipulation here if needed.
            throw new NotImplementedException("This method is not yet implemented for DOCX files.");
        }

        private byte[] UpdateSkillsInPdf(byte[] fileData, List<string> newSkills)
        {
            using (PdfReader reader = new PdfReader(fileData))
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (PdfStamper stamper = new PdfStamper(reader, outputStream))
                {
                    // Iterate over each page in the PDF
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        var strategy = new SimpleTextExtractionStrategy();
                        string pageContent = PdfTextExtractor.GetTextFromPage(reader, i, strategy);

                        // Use a regex pattern to locate and extract the "Web Development" section
                        string webDevPattern = @"Web Development:.*?(?=Work Experience|$)";  // Adjust this based on the actual PDF structure
                        Match match = Regex.Match(pageContent, webDevPattern, RegexOptions.Singleline);

                        if (match.Success)
                        {
                            // Extract the current skills in the "Web Development" section
                            string existingSkills = match.Value;
                            string cleanedSkills = existingSkills.Replace("Web Development:", "").Trim();

                            // Combine the old and new skills
                            string combinedSkills = cleanedSkills + ", " + string.Join(", ", newSkills);

                            // Set the new content back in the same position with the same font
                            PdfContentByte canvas = stamper.GetOverContent(i);
                            canvas.BeginText();

                            // Use the same font and size (you can adjust this based on the actual font used in the original section)
                            BaseFont font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                            canvas.SetFontAndSize(font, 12);  // You may need to adjust this size to match the document

                            // Position the text in the PDF (adjust this based on the actual coordinates in the PDF)
                            canvas.SetTextMatrix(100, 500);  // Adjust based on where the "Web Development" section starts
                            canvas.ShowText("Web Development: " + combinedSkills);
                            canvas.EndText();

                            break; // Exit after modifying the page with the "Web Development" section
                        }
                    }
                }

                return outputStream.ToArray();
            }
        }
    }
}

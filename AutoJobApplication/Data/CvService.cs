using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using AutoJobApplication.Interfaces;

namespace AutoJobApplication.Data
{
    public class CvService : ICvService
    {
        public byte[] AddSkillsToDocx(byte[] docxData, List<string> skills)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(docxData, 0, docxData.Length);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(memoryStream, true))
                {
                    AddSkillsToDocument(doc, skills);
                    doc.MainDocumentPart.Document.Save(); // Save changes to the document
                }
                return memoryStream.ToArray(); // Return the modified document as a byte array
            }
        }

        private void AddSkillsToDocument(WordprocessingDocument doc, List<string> skills)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            Paragraph para = body.AppendChild(new Paragraph());
            Run run = para.AppendChild(new Run());
            run.AppendChild(new Text("Skills: " + string.Join(", ", skills)));
        }
    }
}

using AutoJobApplication.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System;

namespace AutoJobApplication.Data
{
    public class FileStateService : IFileStateService
    {
        public IBrowserFile UploadedFile { get; private set; }

        public void SetUploadedFile(IBrowserFile file)
        {
            Console.WriteLine("Setting uploaded file in FileStateService.");
            UploadedFile = file;
        }

        public void ClearFile()
        {
            Console.WriteLine("Clearing file in FileStateService.");
            UploadedFile = null;
        }
    }
}

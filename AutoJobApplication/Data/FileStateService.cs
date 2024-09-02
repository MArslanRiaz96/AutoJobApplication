using AutoJobApplication.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace AutoJobApplication.Data
{
    public class FileStateService : IFileStateService
    {
        public IBrowserFile UploadedFile { get; private set; }
        public byte[] UploadedFileData { get; set; } // Ensure this property exists

        public void SetUploadedFile(IBrowserFile file)
        {
            UploadedFile = file;
        }

        public void ClearFile()
        {
            UploadedFile = null;
            UploadedFileData = null; // Clear the uploaded file data
        }
    }
}

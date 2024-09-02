using Microsoft.AspNetCore.Components.Forms;

namespace AutoJobApplication.Interfaces
{
    public interface IFileStateService
    {
        IBrowserFile UploadedFile { get; }
        byte[] UploadedFileData { get; set; } // Ensure this is included
        void SetUploadedFile(IBrowserFile file);
        void ClearFile();
    }
}

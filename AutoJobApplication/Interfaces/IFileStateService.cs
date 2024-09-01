using Microsoft.AspNetCore.Components.Forms;

namespace AutoJobApplication.Interfaces
{
    public interface IFileStateService
    {
        IBrowserFile UploadedFile { get; }
        void SetUploadedFile(IBrowserFile file);
        void ClearFile();
    }
}

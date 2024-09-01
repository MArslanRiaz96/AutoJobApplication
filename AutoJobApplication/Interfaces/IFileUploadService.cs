using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace AutoJobApplication.Interfaces
{
    public interface IFileUploadService
    {
        Task<byte[]> UploadFileAsync(IBrowserFile file);
    }
}

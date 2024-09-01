using AutoJobApplication.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using System.Threading.Tasks;

namespace AutoJobApplication.Data
{
    public class FileUploadService : IFileUploadService
    {
        public async Task<byte[]> UploadFileAsync(IBrowserFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(memoryStream); // Adjust size limit as needed
            return memoryStream.ToArray(); // Return the byte array
        }
    }
}

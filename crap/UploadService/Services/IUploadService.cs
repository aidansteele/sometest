using System.Threading.Tasks;
using UploadService.Models;

namespace UploadService.Services
{
    public interface IUploadService
    {
        Task<WriteResponse> UploadFile(FileUploadMessage fileUploadMessage);
    }
}

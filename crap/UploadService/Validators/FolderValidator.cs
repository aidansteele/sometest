using System.Net;
using UploadService.Constants;
using UploadService.Models;
using UploadService.Predicates;

namespace UploadService.Validators
{
    public static class FolderValidator
    {
        public static void EnsureFolderExists(this File file)
        {
            file.FolderExists().ThrowUploadExceptionIfFalse(
                ValidationMessages.ParentFolderDoesNotExistErrorMessage,
                HttpStatusCode.NotFound);
        }
    }
}

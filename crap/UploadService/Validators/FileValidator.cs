using System.Net;
using UploadService.Constants;
using UploadService.Models;
using UploadService.Predicates;

namespace UploadService.Validators
{
    public static class FileValidator
    {
        public static File EnsureFileDoesNotExist(this File file)
        {
            file.FileExists().ThrowUploadExceptionIfTrue(ValidationMessages.FileAlreadyExists, HttpStatusCode.BadRequest);
            return file;
        }

        public static File EnsureFileDoesExist(this File file)
        {
            file.FileExists().ThrowUploadExceptionIfFalse(ValidationMessages.FileDoesNotExist, HttpStatusCode.BadRequest);
            return file;
        }
    }
}

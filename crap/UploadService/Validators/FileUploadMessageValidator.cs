using System.Net;
using UploadService.Constants;
using UploadService.Models;

namespace UploadService.Validators
{
    public static class FileUploadMessageValidator
    {
        public static void Validate(this FileUploadMessage fileUploadMessage)
        {
            fileUploadMessage.EnsureIsPresent()
                .EnsureMetadataIsPresent()
                .EnsureContentIsPresent();

            fileUploadMessage.Metadata
                .EnsureFileIsNotEmpty()
                .EnsureFileDoesNotExceedMaxFileSize()
                .EnsureFilenameIsPresent()
                .EnsureFilenameDoesNotExceedMaxLength()
                .EnsureValidFileExtension()
                .EnsureFilenameContainsValidChars();
        }

        public static FileUploadMessage EnsureIsPresent(this FileUploadMessage fileUploadMessage)
        {
            fileUploadMessage.ThrowUploadExceptionIfNull(
                ValidationMessages.FileUploadMessageIsNullErrorMessage,
                HttpStatusCode.BadRequest);

            return fileUploadMessage;
        }

        public static FileUploadMessage EnsureMetadataIsPresent(this FileUploadMessage fileUploadMessage)
        {
            fileUploadMessage.Metadata.ThrowUploadExceptionIfNull(
                ValidationMessages.FileMetadataIsNullErrorMessage,
                HttpStatusCode.BadRequest);

            return fileUploadMessage;
        }

        public static FileUploadMessage EnsureContentIsPresent(this FileUploadMessage fileUploadMessage)
        {
            fileUploadMessage.Content.ThrowUploadExceptionIfNull(
                ValidationMessages.FileContentIsNullErrorMessage,
                HttpStatusCode.BadRequest);

            return fileUploadMessage;
        }
    }
}

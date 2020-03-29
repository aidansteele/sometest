using System.Net;
using UploadService.Constants;
using UploadService.Models;
using UploadService.Predicates;

namespace UploadService.Validators
{
    public static class FileMetadataValidator
    {
        public static FileMetadata EnsureFileIsNotEmpty(this FileMetadata fileMetaData)
        {
            fileMetaData.IsEmptyFile()
                .ThrowUploadExceptionIfTrue(ValidationMessages.EmptyFileErrorMessage, HttpStatusCode.BadRequest);

            return fileMetaData;
        }

        public static FileMetadata EnsureFileDoesNotExceedMaxFileSize(this FileMetadata fileMetaData)
        {
            fileMetaData.ExceedsMaximumFileSize().ThrowUploadExceptionIfTrue(
                ValidationMessages.ExceedsMaximumFileSizeErrorMessage, HttpStatusCode.RequestEntityTooLarge);

            return fileMetaData;
        }

        public static FileMetadata EnsureFilenameIsPresent(this FileMetadata fileMetaData)
        {
            fileMetaData.IsEmptyFileName()
                .ThrowUploadExceptionIfTrue(ValidationMessages.EmptyFileNameErrorMessage, HttpStatusCode.BadRequest);

            return fileMetaData;
        }

        public static FileMetadata EnsureFilenameContainsValidChars(this FileMetadata fileMetadata)
        {
            fileMetadata.IsValidFilename()
                .ThrowUploadExceptionIfFalse(ValidationMessages.InvalidFilename, HttpStatusCode.BadRequest);

            return fileMetadata;
        }

        public static FileMetadata EnsureFilenameDoesNotExceedMaxLength(this FileMetadata fileMetaData)
        {
            fileMetaData.ExceedsMaximumFileNameLength()
                .ThrowUploadExceptionIfTrue(
                    ValidationMessages.ExceedFileNameLengthErrorMessage,
                    HttpStatusCode.BadRequest);

            return fileMetaData;
        }

        public static FileMetadata EnsureValidFileExtension(this FileMetadata fileMetaData)
        {
            fileMetaData.IsValidExtension()
                .ThrowUploadExceptionIfFalse(
                    ValidationMessages.InvalidFileExtensionErrorMessage,
                    HttpStatusCode.UnsupportedMediaType);

            return fileMetaData;
        }
    }
}

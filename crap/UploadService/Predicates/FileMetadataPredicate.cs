using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UploadService.Constants;
using UploadService.Models;
using static UploadService.Constants.FileEnums;

namespace UploadService.Predicates
{
    public static class FileMetadataPredicate
    {
        public static bool ExceedsMaximumFileSize(this FileMetadata fileMetadata)
        {
            return fileMetadata.Size >= (int)FileSize.Maximum;
        }

        public static bool IsEmptyFile(this FileMetadata fileMetadata)
        {
            return fileMetadata.Size <= (int)FileSize.Empty;
        }

        public static bool IsValidExtension(this FileMetadata fileMetadata)
        {
            var extension = Path.GetExtension(fileMetadata.Name);
            var regex = new Regex(
                string.Join("|", MimeType.ValidMimeTypes.Select(mt => @"^\." + mt + "$").ToArray()),
                RegexOptions.IgnoreCase);

            return extension != null && regex.IsMatch(extension);
        }

        public static bool IsValidFilename(this FileMetadata fileMetadata)
        {
            var invalidChars = "\\/:*?\"<>|".ToCharArray();

            return !fileMetadata.Name.Any(c => invalidChars.Contains(c))
                && !string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(fileMetadata.Name));
        }

        public static bool ExceedsMaximumFileNameLength(this FileMetadata fileMetadata)
        {
            return fileMetadata.Name.Length > (int)FileNameLength.Maximum;
        }

        public static bool IsEmptyFileName(this FileMetadata fileMetadata)
        {
            return string.IsNullOrWhiteSpace(fileMetadata.Name);
        }
    }
}

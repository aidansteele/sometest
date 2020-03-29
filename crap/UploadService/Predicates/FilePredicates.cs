using UploadService.Constants;
using UploadService.Models;

namespace UploadService.Predicates
{
    public static class FilePredicates
    {
        public static bool FolderExists(this File file)
        {
            return file != null && file.StatusCode != FileStatus.Deleted && file.Folder;
        }

        public static bool FileExists(this File file)
        {
            return file != null && file.StatusCode == FileStatus.Uploaded && !file.Folder;
        }
    }
}

using System.IO;

namespace UploadService.Models
{
    public class FileUploadMessage
    {
        public FileMetadata Metadata { get; set; }
        public Stream Content { get; set; }
    }
}

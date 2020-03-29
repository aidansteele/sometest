using System;

namespace UploadService.Models
{
    // todo: clean up not required properties
    public class File
    {
        public Guid FileId { get; set; }
        public string StatusCode { get; set; }
        public bool Folder { get; set; }
    }
}

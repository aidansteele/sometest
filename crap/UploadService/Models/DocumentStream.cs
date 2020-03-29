using System;
using System.IO;

namespace UploadService.Models
{
    public class DocumentStream
    {
        public Stream Content { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganisationId { get; set; }
    }
}

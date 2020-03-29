using System;
using UploadService.Constants;

namespace UploadService.Models
{
    public class FileMetadata
    {
        public Guid FileId { get; set; }
        public Guid DocumentServiceId { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public long CompressedSize { get; set; }
        public string FileStatus { get; set; }
        public Guid? AssociateWithId { get; set; }
        public string AssociateWithType { get; set; }
        public string AssociationStatus { get; set; }
        public DateTime? CreatedDateUTC { get; set; }
        public Guid? EmailId { get; set; }
        public string FileSource { get; set; }
        public byte[] EncryptedKey { get; set; }
        public byte[] InitialisationVector { get; set; }
        public byte[] HashOfUploadedFile { get; set; }
        public CompressionMethod CompressionMethod { get; set; }
        public EncryptionMethod EncryptionMethod { get; set; }
    }
}

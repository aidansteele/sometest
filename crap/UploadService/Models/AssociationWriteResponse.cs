using System;

namespace UploadService.Models
{
    public class AssociationWriteResponse
    {
        public Guid OrganisationId { get; set; }
        public Guid Id { get; set; }
        public Guid FileId { get; set; }
        public string ObjectType { get; set; }
    }
}

using System;

namespace UploadService.Models
{
    public class AssociateFile
    {
        public Guid OrganisationId { get; set; }
        public Guid ObjectId { get; set; }
        public Guid FileId { get; set; }
        public string ObjectType { get; set; }
        public string AssociationStatusCode { get; set; }
        public string AssociationTypeCode { get; set; }
        public DateTime? CreatedDateUtc { get; set; }
        public DateTime? UpdatedDateUtc { get; set; }
    }
}

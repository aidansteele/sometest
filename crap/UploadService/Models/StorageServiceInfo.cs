using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadService.Models
{
    public class StorageServiceInfo
    {
        public Guid OrganisationId { get; set; }
        public string Email { get; set; }
        public Guid RootFileId { get; set; }
        public Guid InboxFileId { get; set; }
        public DateTime CreatedDateUTC { get; set; }
        public DateTime UpdatedDateUTC { get; set; }
        public string InboxEmailAddress { get; set; }
        public string ShortCode { get; set; }
    }
}

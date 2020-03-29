using System;

namespace UploadService.Models
{
    public class CurrentUserContext
    {
        public Guid UserId { get; set; }
        public Guid OrganisationId { get; set; }
    }
}

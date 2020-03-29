using System;
using System.ComponentModel.DataAnnotations;

namespace UploadService.Models
{
    public class AssociationCreation
    {
        [Required]
        public Guid AssociateWithId { get; set; }

        [Required]
        public Guid FileId { get; set; }

        [Required]
        public string ObjectType { get; set; }
    }
}

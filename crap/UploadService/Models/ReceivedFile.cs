using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using UploadService.Converters;

namespace UploadService.Models
{
    public class ReceivedFile
    {
        [Required]
        public Guid FileId { get; set; }

        public Guid? FolderId { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public long ContentLength { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Required]
        public byte[] FileBytes { get; set; }
    }
}

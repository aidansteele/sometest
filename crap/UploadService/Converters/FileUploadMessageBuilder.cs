using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using UploadService.Models;

namespace UploadService.Converters
{
    public static class FileUploadMessageBuilder
    {
        public static FileUploadMessage Init()
        {
            return new FileUploadMessage();
        }

        public static FileUploadMessage FromBody(this FileUploadMessage fum, ReceivedFile file)
        {
            fum.Metadata = new FileMetadata()
            {
                FileId = file.FileId == Guid.Empty ? Guid.NewGuid() : file.FileId,
                Size = file.ContentLength,
                Name = file.FileName,
                MimeType = file.ContentType
            };

            fum.Content = new MemoryStream(file.FileBytes);
            return fum;
        }

        public static FileUploadMessage WithIdParameters(
            this FileUploadMessage fum,
            Guid userId,
            Guid orgId,
            Guid? parentId)
        {
            fum.Metadata.UserId = userId;
            fum.Metadata.OrganisationId = orgId;
            fum.Metadata.ParentId = parentId;
            return fum;
        }
    }
}

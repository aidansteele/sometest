using System;
using System.Threading.Tasks;
using UploadService.Connectors;
using UploadService.Constants;
using UploadService.Models;
using UploadService.Resolver;
using UploadService.Validators;

namespace UploadService.Services
{
    public class UploadService : IUploadService
    {
        private readonly IXeroDbConnector _xeroDbConnector;
        private readonly IDocumentServiceConnector _documentServiceConnector;

        public UploadService(IXeroDbConnector xeroDbConnector, IDocumentServiceConnector documentServiceConnector)
        {
            _xeroDbConnector = xeroDbConnector;
            _documentServiceConnector = documentServiceConnector;
        }

        public async Task<WriteResponse> UploadFile(FileUploadMessage fileUploadMessage)
        {
            fileUploadMessage.Validate();

            var metadata = fileUploadMessage.Metadata;
            var orgId = metadata.OrganisationId;

            var storageServiceInfo = await _xeroDbConnector.GetStorageServiceInfo(orgId);
            storageServiceInfo.EnsureOrgIsProvisioned();

            var file = await _xeroDbConnector.GetFileAsync(orgId, metadata.FileId);
            file.EnsureFileDoesNotExist();

            metadata.ParentId = metadata.ResolveParentId(storageServiceInfo);

            var parent = await _xeroDbConnector.GetFileAsync(orgId, metadata.ParentId.Value);
            parent.EnsureFolderExists();

            var response = await _documentServiceConnector.PostDocumentAsync(new DocumentStream()
            {
                UserId = metadata.UserId,
                Content = fileUploadMessage.Content,
                OrganisationId = orgId,
                FileName = metadata.Name,
                MimeType = metadata.MimeType
            });

            metadata.DocumentServiceId = response.Id;
            metadata.CompressionMethod = CompressionMethod.None;
            metadata.EncryptionMethod = EncryptionMethod.None;

            await SaveNewFile(metadata);

            return new WriteResponse()
            {
                FileId = fileUploadMessage.Metadata.FileId
            };
        }

        private async Task SaveNewFile(FileMetadata metadata)
        {
            var fileParams = new
            {
                FileId = metadata.FileId,
                Name = metadata.Name,
                OrganisationId = metadata.OrganisationId,
                ParentId = metadata.ParentId,
                CreatedDateUTC = metadata.CreatedDateUTC ?? DateTime.UtcNow,
                UpdatedDateUTC = metadata.CreatedDateUTC ?? DateTime.UtcNow,
                MimeType = metadata.MimeType,
                UserId = metadata.UserId,
                Size = metadata.Size,
                StatusCode = metadata.FileStatus ?? "FILE/UPLOADED",
                IsSystem = false,
                EmailID = metadata.EmailId,
                EncryptedKey = metadata.EncryptedKey,
                InitialisationVector = metadata.InitialisationVector,
                HashOfUploadedFile = metadata.HashOfUploadedFile,
                CompressionMethod = metadata.CompressionMethod,
                EncryptionMethod = metadata.EncryptionMethod,
                FileSourceCode = metadata.FileSource,
                CompressedSize = metadata.CompressedSize,
                DocumentServiceID = metadata.DocumentServiceId
            };
            await _xeroDbConnector.SaveNewFile(fileParams);
        }
    }
}

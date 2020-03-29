using System;
using System.Threading.Tasks;
using UploadService.Models;

namespace UploadService.Connectors
{
    public interface IXeroDbConnector
    {
        Task<File> GetFileAsync(Guid orgId, Guid fileId);
        Task<bool> DoesFileExist(Guid organisationId, Guid fileId);
        Task UpdateFile<T>(T fileParams);
        Task SaveNewFile<T>(T fileParams);
        Task<StorageServiceInfo> GetStorageServiceInfo(Guid organisationId);
        Task AssociateFile<T>(T fileAssociationParams);
    }
}

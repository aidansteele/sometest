using System;
using UploadService.Models;

namespace UploadService.Resolver
{
    public static class ParentFolderIdResolver
    {
        public static Guid? ResolveParentId(this FileMetadata metadata, StorageServiceInfo storageServiceInfo)
        {
            return !metadata.ParentId.HasValue ||
                   metadata.ParentId.Value == Guid.Empty ||
                   metadata.ParentId.Equals(storageServiceInfo?.RootFileId)
                ? storageServiceInfo?.InboxFileId
                : metadata.ParentId;
        }
    }
}

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using UploadService.Configurations;
using UploadService.Models;

namespace UploadService.Connectors
{
    public class XeroDbConnector : IXeroDbConnector
    {
        private readonly XeroDocsConfiguration _config;

        public XeroDbConnector(IOptions<XeroDocsConfiguration> config)
        {
            _config = config.Value;
        }

        public async Task<File> GetFileAsync(Guid orgId, Guid fileId)
        {
            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                var file = (await connection.QueryAsync<File>(
                    @"
                    Set NoCount On;

                    Select 
                        F.FileID, F.StatusCode, F.Folder
                    From
						dbo.[File] F (NoLock)
                    Where 
                        OrganisationID = @OrganisationID And
                        FileID = @FileID",
                    new
                    {
                        OrganisationID = orgId,
                        FileID = fileId
                    })).FirstOrDefault();
                return file;
            }
        }

        public async Task<bool> DoesFileExist(Guid organisationId, Guid fileId)
        {
            var result = await GetFileAsync(organisationId, fileId);
            return result != null;
        }

        public async Task UpdateFile<T>(T fileParams)
        {
            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                string sql = @"
                    Set NoCount On;

                    Update
                        dbo.[File] 
                    Set 
                        Name = @FileName,
                        UpdatedDateUTC = @UpdatedDateUTC,
                        ParentId = @ParentId,
                        MimeType = @MimeType,
                        Size = @Size,
                        EncryptedKey = @EncryptedKey,
                        InitialisationVector = @InitialisationVector,
                        HashOfUploadedFile= @HashOfUploadedFile,
                        CompressionMethod = @CompressionMethod,
                        EncryptionMethod = @EncryptionMethod,
                        CompressedSize = @CompressedSize,
                        DocumentServiceID = @DocumentServiceID
                    Where
                        OrganisationID = @OrganisationID And
                        FileID = @FileID
                    ";
                await connection.ExecuteAsync(sql, fileParams);
            }
        }

        public async Task SaveNewFile<T>(T fileParams)
        {
            using (var conn = new SqlConnection(_config.ConnectionString))
            {
                await conn.ExecuteAsync(
                    @"
					Set NoCount On;
					Insert dbo.[File](
                        FileId,
                        Name,
                        OrganisationId,
                        ParentId,
                        Folder,
                        CreatedDateUTC,
                        UpdatedDateUTC,
                        MimeType,
                        UserId,
                        Size,
                        StatusCode,
                        IsSystem,
                        EmailID,
                        EncryptedKey,
                        InitialisationVector,
                        HashOfUploadedFile,
                        CompressionMethod,
                        EncryptionMethod,
                        FileSourceCode,
                        CompressedSize,
                        DocumentServiceID)
                    Values (
                        @FileId,
                        @Name,
                        @OrganisationId,
                        @ParentId,
                        0,
                        @CreatedDateUTC,
                        @UpdatedDateUTC,
                        @MimeType,
                        @UserId,
                        @Size,
                        @StatusCode,
                        @IsSystem,
                        @EmailID,
                        @EncryptedKey,
                        @InitialisationVector,
                        @HashOfUploadedFile,
                        @CompressionMethod,
                        @EncryptionMethod,
                        @FileSourceCode,
                        @CompressedSize,
                        @DocumentServiceID)
				    ",
                    fileParams);
            }
        }

        public async Task<StorageServiceInfo> GetStorageServiceInfo(Guid organisationId)
        {
            using (var connection = new SqlConnection(_config.ConnectionString))
            {
                var result = await connection.QueryAsync<StorageServiceInfo>(
                    @"
                    Set NoCount On; 
                    Select * From dbo.[StorageServiceInfo] (NoLock)
                    Where OrganisationId = @OrgId;
                    ",
                    new { OrgId = organisationId });

                return result.FirstOrDefault();
            }
        }

        public async Task AssociateFile<T>(T fileAssociationParams)
        {
            using (var conn = new SqlConnection(_config.ConnectionString))
            {
                await conn.ExecuteAsync(
                    @"
						Set NoCount On;
						Merge 
							dbo.[FileObjectAssociation] AS target
						Using 
							(SELECT @OrganisationId, @ObjectId, @FileId) AS source(OrganisationId, ObjectId, FileId)
						On 
							target.OrganisationID = source.OrganisationId And 
							target.ObjectID = source.ObjectId And
							target.FileID = source.FileId
						When Matched Then
							Update Set 
								AssociationTypeCode = @AssociationTypeCode,
								AssociationStatusCode = @AssociationStatusCode,
								UpdatedDateUTC = @UpdatedDateUTC
						When Not Matched Then 
								Insert 
									(OrganisationID, ObjectType, ObjectId, FileId, AssociationTypeCode, AssociationStatusCode, CreatedDateUTC, UpdatedDateUTC)
								Values (
									@OrganisationId, @ObjectType, @ObjectId, @FileId, @AssociationTypeCode, @AssociationStatusCode, @CreatedDateUTC, @UpdatedDateUTC
								);
						If(@AssociationStatusCode = 'ASSOCIATIONSTATUS/ACTIVE')
							Begin
								Declare @InboxID uniqueidentifier = (Select InboxFileID From dbo.StorageServiceInfo (NoLock) Where OrganisationID = @OrganisationId)
								Update
									dbo.[File]
								Set
									ParentID = Null
								Where
									OrganisationID = @OrganisationId And
									FileID = @FileId And
									ParentID = @InboxID
							End
                    ",
                    fileAssociationParams);
            }
        }
    }
}

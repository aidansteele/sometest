using System;
using UploadService.Models;

namespace UploadService.Converters
{
    public static class AssociateFileConverter
    {
        public static AssociateFile Convert(this AssociationCreation payload, Guid organisationId)
        {
            return new AssociateFile
            {
                OrganisationId = organisationId,
                ObjectId = payload.AssociateWithId,
                FileId = payload.FileId,
                ObjectType = payload.ObjectType,
                AssociationStatusCode = "ASSOCIATIONSTATUS/ACTIVE",
                AssociationTypeCode = "ASSOCIATIONTYPE/DIRECT",
                CreatedDateUtc = DateTime.UtcNow,
                UpdatedDateUtc = DateTime.UtcNow
            };
        }
    }
}

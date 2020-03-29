using System;
using System.Threading.Tasks;
using UploadService.Connectors;
using UploadService.Converters;
using UploadService.Models;
using UploadService.Validators;

namespace UploadService.Services
{
    public class AssociationService : IAssociationService
    {
        private readonly IXeroDbConnector _xeroDbConnector;

        public AssociationService(IXeroDbConnector xeroDbConnector)
        {
            _xeroDbConnector = xeroDbConnector;
        }

        public async Task<AssociationWriteResponse> AddAssociation(Guid organisationId, AssociationCreation payload)
        {
            payload.Validate();

            var storageServiceInfo = await _xeroDbConnector.GetStorageServiceInfo(organisationId);
            storageServiceInfo.EnsureOrgIsProvisioned();

            var file = await _xeroDbConnector.GetFileAsync(organisationId, payload.FileId);
            file.EnsureFileDoesExist();

            await _xeroDbConnector.AssociateFile(payload.Convert(organisationId));

            return new AssociationWriteResponse
            {
                OrganisationId = organisationId,
                Id = payload.AssociateWithId,
                FileId = payload.FileId,
                ObjectType = payload.ObjectType
            };
        }
    }
}

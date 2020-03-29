using System;
using System.Threading.Tasks;
using UploadService.Models;

namespace UploadService.Services
{
    public interface IAssociationService
    {
        Task<AssociationWriteResponse> AddAssociation(Guid organisationId, AssociationCreation payload);
    }
}

using UploadService.Constants;
using UploadService.Models;

namespace UploadService.Validators
{
    public static class StorageServiceInfoValidator
    {
        public static StorageServiceInfo EnsureOrgIsProvisioned(this StorageServiceInfo ssi)
        {
            ssi.ThrowUploadExceptionIfNull(
                ValidationMessages.OrganisationNotProvisionedErrorMessage);

            return ssi;
        }
    }
}

using System.Net;
using UploadService.Constants;
using UploadService.Models;
using UploadService.Predicates;

namespace UploadService.Validators
{
    public static class AssociationValidator
    {
        public static void Validate(this AssociationCreation payload)
        {
            payload.EnsureFileIdObjectIdIsNotNull();
            payload.EnsureObjectTypeIsValid();
        }

        public static AssociationCreation EnsureFileIdObjectIdIsNotNull(this AssociationCreation associationCreation)
        {
            associationCreation.IsEmptyFileIdAndEmptyAssociateId().ThrowUploadExceptionIfTrue(ValidationMessages.FieldCannotBeEmpty, HttpStatusCode.BadRequest);

            return associationCreation;
        }

        public static AssociationCreation EnsureObjectTypeIsValid(this AssociationCreation associationCreation)
        {
            associationCreation.IsValidObjectType().ThrowUploadExceptionIfFalse(ValidationMessages.InvalidObjectType, HttpStatusCode.BadRequest);

            return associationCreation;
        }
    }
}

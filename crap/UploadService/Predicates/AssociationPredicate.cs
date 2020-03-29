using System;
using System.Text.RegularExpressions;
using UploadService.Models;

namespace UploadService.Predicates
{
    public static class AssociationPredicate
    {
        public static bool IsEmptyFileIdAndEmptyAssociateId(this AssociationCreation associationCreation)
        {
            return associationCreation.FileId == Guid.Empty || associationCreation.AssociateWithId == Guid.Empty;
        }

        public static bool IsValidObjectType(this AssociationCreation associationCreation)
        {
            Regex validCharacters = new Regex("^[a-zA-Z0-9/]+$");
            return !string.IsNullOrWhiteSpace(associationCreation.ObjectType) && validCharacters.IsMatch(associationCreation.ObjectType);
        }
    }
}

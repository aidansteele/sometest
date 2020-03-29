namespace UploadService.Constants
{
    public class ValidationMessages
    {
        public const string InvalidOrganisationIdErrorMessage = "Organisation Id is not provided.";
        public const string EmptyFileErrorMessage = "File cannot be empty.";
        public const string ExceedsMaximumFileSizeErrorMessage = "File size is too large.";
        public const string InvalidFileExtensionErrorMessage = "Unsupported file extension.";
        public const string ParentFolderDoesNotExistErrorMessage = "Parent folder not found.";
        public const string OrganisationNotProvisionedErrorMessage = "Organisation has not yet been provisioned in Files.";
        public const string FileUploadMessageIsNullErrorMessage = "File upload message is null.";
        public const string AssociationsPayloadIsNullErrorMessage = "File association payload is null.";
        public const string FileMetadataIsNullErrorMessage = "File metadata is null.";
        public const string FileContentIsNullErrorMessage = "File content is null.";
        public const string EmptyFileNameErrorMessage = "File name cannot be empty.";
        public const string ExceedFileNameLengthErrorMessage = "File name exceeds maximum length 512.";
        public const string InvalidFilename = "File name cannot contain reserved characters '<>:\"/\\|?*'";
        public const string FileAlreadyExists = "File already exists.";
        public const string FileDoesNotExist = "File does not exist.";
        public const string InvalidObjectType = "Object type is invalid.";
        public const string ObjectTypeIsNullErrorMessage = "Object type cannot be null";
        public const string FieldCannotBeEmpty = "Field cannot be empty";
    }
}

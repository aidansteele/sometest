using System;
using System.Net;
using UploadService.Constants;
using UploadService.Models;
using UploadService.Models.Exceptions;
using UploadService.Validators;
using Xunit;

namespace UploadService.Tests.Unit
{
    public class FolderValidatorTests
    {
        [Fact]
        public void GivenParentFolderIsNull_ExpectNotFound()
        {
            // Arrange
            // Act
            Exception ex = Assert.Throws<UploadException>(() => FolderValidator.EnsureFolderExists(null));

            // Assert
            Assert.Equal(typeof(UploadException), ex.GetType());
            Assert.Equal(ValidationMessages.ParentFolderDoesNotExistErrorMessage, ex.Message);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)((UploadException)ex).StatusCode);
        }

        [Fact]
        public void GivenParentFolderIsDeleted_ExpectNotFound()
        {
            // Arrange
            File file = new File()
            {
                FileId = Guid.NewGuid(),
                StatusCode = FileStatus.Deleted,
                Folder = true
            };

            // Act
            Exception ex = Assert.Throws<UploadException>(() => file.EnsureFolderExists());

            // Assert
            Assert.Equal(typeof(UploadException), ex.GetType());
            Assert.Equal(ValidationMessages.ParentFolderDoesNotExistErrorMessage, ex.Message);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)((UploadException)ex).StatusCode);
        }

        [Fact]
        public void GivenParentFolderDoesExist_ExpectVoid()
        {
            // Arrange
            File file = new File()
            {
                FileId = Guid.NewGuid(),
                StatusCode = null,
                Folder = true
            };

            // Act
            var exception = Record.Exception(() => file.EnsureFolderExists());

            // Assert
            Assert.Equal(null, exception);
        }

        [Fact]
        public void GivenFileIsNotAFolder_ExpectNotFound()
        {
            // Arrange
            File file = new File()
            {
                FileId = Guid.NewGuid(),
                StatusCode = null,
                Folder = false
            };

            // Act
            var ex = Record.Exception(() => file.EnsureFolderExists());

            // Assert
            Assert.Equal(typeof(UploadException), ex.GetType());
            Assert.Equal(ValidationMessages.ParentFolderDoesNotExistErrorMessage, ex.Message);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)((UploadException)ex).StatusCode);
        }
    }
}

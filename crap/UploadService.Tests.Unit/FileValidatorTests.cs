using System;
using System.Net;
using UploadService.Constants;
using UploadService.Models;
using UploadService.Models.Exceptions;
using UploadService.Validators;
using Xunit;

namespace UploadService.Tests.Unit
{
    public class FileValidatorTests
    {
        [Fact]
        public void GivenFileIsNull_ExpectBadRequest()
        {
            // Arrange
            // Act
            Exception ex = Assert.Throws<UploadException>(() => FileValidator.EnsureFileDoesExist(null));

            // Assert
            Assert.Equal(typeof(UploadException), ex.GetType());
            Assert.Equal(ValidationMessages.FileDoesNotExist, ex.Message);
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)((UploadException)ex).StatusCode);
        }

        [Fact]
        public void GivenFileHasBeenDeleted_ExpectNoException()
        {
            // Arrange
            File file = new File
            {
                FileId = Guid.NewGuid(),
                StatusCode = FileStatus.Deleted,
                Folder = false
            };

            // Act
            file.EnsureFileDoesNotExist();
        }

        [Fact]
        public void Given_FileWithNullStatusCode_ExpectNoException()
        {
            // Arrange
            File file = new File
            {
                FileId = Guid.NewGuid(),
                StatusCode = null,
                Folder = false
            };

            // Act
            file.EnsureFileDoesNotExist();
        }

        [Fact]
        public void GivenFileWithUploadedStatus_ExpectNoException()
        {
            // Arrange
            File file = new File
            {
                FileId = Guid.NewGuid(),
                StatusCode = FileStatus.Uploaded,
                Folder = false
            };

            // Act
            file.EnsureFileDoesExist();
        }
    }
}

using System;
using System.IO;
using System.Net;
using UploadService.Constants;
using UploadService.Models;
using UploadService.Models.Exceptions;
using UploadService.Tests.Unit.Factory;
using UploadService.Validators;
using Xunit;

namespace UploadService.Tests.Unit
{
    public class FileMetadataValidatorTests
    {
        [Fact]
        public void GivenAnEmptyFile_ExpectBadRequest()
        {
            using (var ms = new MemoryStream())
            {
                // Arrange
                var emptyFileSize = 0;
                var messageWithEmptySize = new FileUploadMessage
                {
                    Metadata = new FileMetadata() { Size = emptyFileSize, Name = "bills.jpg" },
                    Content = ms
                };

                // Act
                Exception ex = Assert.Throws<UploadException>(() => messageWithEmptySize.Validate());

                // Assert
                Assert.Equal(typeof(UploadException), ex.GetType());
                Assert.Equal(ValidationMessages.EmptyFileErrorMessage, ex.Message);
                Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)((UploadException)ex).StatusCode);
            }
        }

        [Fact]
        public void GivenAFileIsNotEmpty_ExpectNoException()
        {
            using (var ms = new MemoryStream())
            {
                // Arrange
                var validFileSize = 1;
                var messageWithValidSize = new FileUploadMessage
                {
                    Metadata = new FileMetadata() { Size = validFileSize, Name = "bills.jpg" },
                    Content = ms
                };

                // Act
                messageWithValidSize.Validate();
            }
        }

        [Fact]
        public void GivenAFileExceedsMaximum_ExpectRequestEntityTooLarge()
        {
            using (var ms = new MemoryStream())
            {
                // Arrange
                const long greaterThanMaximumfileSize = 26214501;
                var messageWithExcessiveSize = new FileUploadMessage
                {
                    Metadata = new FileMetadata() { Size = greaterThanMaximumfileSize, Name = "bills.jpg" },
                    Content = ms
                };

                // Act
                Exception ex = Assert.Throws<UploadException>(() => messageWithExcessiveSize.Validate());

                // Assert
                Assert.Equal(typeof(UploadException), ex.GetType());
                Assert.Equal(ValidationMessages.ExceedsMaximumFileSizeErrorMessage, ex.Message);
                Assert.Equal(HttpStatusCode.RequestEntityTooLarge, (HttpStatusCode)((UploadException)ex).StatusCode);
            }
        }

        [Fact]
        public void GivenAFileWithInvalidExtension_ExpectUnsupportedMediaType()
        {
            using (var ms = new MemoryStream())
            {
                // Arrange
                var invalidFileName = "bills.abc";
                var messageWithInvalidExtension = new FileUploadMessage
                {
                    Metadata = new FileMetadata() { Size = 1, Name = invalidFileName },
                    Content = ms
                };

                // Act
                Exception ex = Assert.Throws<UploadException>(() => messageWithInvalidExtension.Validate());

                // Assert
                Assert.Equal(typeof(UploadException), ex.GetType());
                Assert.Equal(ValidationMessages.InvalidFileExtensionErrorMessage, ex.Message);
                Assert.Equal(HttpStatusCode.UnsupportedMediaType, (HttpStatusCode)((UploadException)ex).StatusCode);
            }
        }

        [Fact]
        public void GivenAFileWithValidExtension_ExpectNoException()
        {
            using (var ms = new MemoryStream())
            {
                // Arrange
                var validFileName = "test.png";
                var messageWithValidExtension = new FileUploadMessage
                {
                    Metadata = new FileMetadata() { Size = 1, Name = validFileName },
                    Content = ms
                };

                // Act
                messageWithValidExtension.Validate();
            }
        }

        [Fact]
        public void GivenAFileWithValidName_ExpectException()
        {
            using (var ms = new MemoryStream())
            {
                // Arrange
                var inValidFileName = ".png";
                var messageWithInvalidFilename = new FileUploadMessage
                {
                    Metadata = new FileMetadata() { Size = 1, Name = inValidFileName },
                    Content = ms
                };

                // Act
                Exception ex = Assert.Throws<UploadException>(() => messageWithInvalidFilename.Validate());

                // Assert
                Assert.Equal(typeof(UploadException), ex.GetType());
                Assert.Equal(ValidationMessages.InvalidFilename, ex.Message);
                Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)((UploadException)ex).StatusCode);
            }
        }

        [Fact]
        public void GivenAFileWithAnEmptyName_ExpectBadRequest()
        {
            using (var ms = new MemoryStream())
            {
                // Arrange
                var emptyFileName = string.Empty;
                var messageWithEmptyFilename = new FileUploadMessage
                {
                    Metadata = new FileMetadata() { Size = 1, Name = emptyFileName },
                    Content = ms
                };

                // Act
                Exception ex = Assert.Throws<UploadException>(() => messageWithEmptyFilename.Validate());

                // Assert
                Assert.Equal(typeof(UploadException), ex.GetType());
                Assert.Equal(ValidationMessages.EmptyFileNameErrorMessage, ex.Message);
                Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)((UploadException)ex).StatusCode);
            }
        }

        [Fact]
        public void GivenAFileWithNameExceedsMaxLength_ExpectBadRequest()
        {
            using (var ms = new MemoryStream())
            {
                // Arrange
                var superLongName = "abc".GenerateRandomString(600).GenerateFileName(".png");
                var messageWithExcessivelyLongFilename = new FileUploadMessage
                {
                    Metadata = new FileMetadata() { Size = 1, Name = superLongName },
                    Content = ms
                };

                // Act
                Exception ex = Assert.Throws<UploadException>(() => messageWithExcessivelyLongFilename.Validate());

                // Assert
                Assert.Equal(typeof(UploadException), ex.GetType());
                Assert.Equal(ValidationMessages.ExceedFileNameLengthErrorMessage, ex.Message);
                Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)((UploadException)ex).StatusCode);
            }
        }
    }
}

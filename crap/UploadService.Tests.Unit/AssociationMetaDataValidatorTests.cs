using System;
using System.Net;
using UploadService.Constants;
using UploadService.Models;
using UploadService.Models.Exceptions;
using UploadService.Validators;
using Xunit;

namespace UploadService.Tests.Unit
{
    public class AssociationMetaDataValidatorTests
    {
        [Fact]
        public void GivenAnAssociationWithValidMetaData_ExpectNoException()
        {
            // Arrange
            var messageWithValidMetaData = new AssociationCreation
            {
                FileId = Guid.NewGuid(),
                AssociateWithId = Guid.NewGuid(),
                ObjectType = "INVOICE"
            };

            // Act
            messageWithValidMetaData.Validate();
        }

        [Fact]
        public void GivenAnEmptyFileId_ExpectBadRequest()
        {
            // Arrange
            var messageWithEmptyFileId = new AssociationCreation
            {
                FileId = Guid.Empty
            };

            // Act
            Exception ex = Assert.Throws<UploadException>(() => messageWithEmptyFileId.Validate());

            // Assert
            Assert.Equal(typeof(UploadException), ex.GetType());
            Assert.Equal(ValidationMessages.FieldCannotBeEmpty, ex.Message);
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)((UploadException)ex).StatusCode);
        }

        [Fact]
        public void GivenAnEmptyAssociatedWithId_ExpectBadRequest()
        {
            // Arrange
            var messageWithEmptyAssociatedWithId = new AssociationCreation
            {
                AssociateWithId = Guid.Empty
            };

            // Act
            Exception ex = Assert.Throws<UploadException>(() => messageWithEmptyAssociatedWithId.Validate());

            // Assert
            Assert.Equal(typeof(UploadException), ex.GetType());
            Assert.Equal(ValidationMessages.FieldCannotBeEmpty, ex.Message);
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)((UploadException)ex).StatusCode);
        }

        [Fact]
        public void GivenAnEmptyObjectType_ExpectBadRequest()
        {
            // Arrange
            var messageWithEmptyObjectType = new AssociationCreation
            {
                FileId = Guid.NewGuid(),
                AssociateWithId = Guid.NewGuid(),
                ObjectType = string.Empty
            };

            // Act
            Exception ex = Assert.Throws<UploadException>(() => messageWithEmptyObjectType.Validate());

            // Assert
            Assert.Equal(typeof(UploadException), ex.GetType());
            Assert.Equal(ValidationMessages.InvalidObjectType, ex.Message);
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)((UploadException)ex).StatusCode);
        }

        [Fact]
        public void GivenANullObjectType_ExpectBadRequest()
        {
            // Arrange
            var messageWithNullObjectType = new AssociationCreation
            {
                FileId = Guid.NewGuid(),
                AssociateWithId = Guid.NewGuid(),
                ObjectType = null
            };

            // Act
            Exception ex = Assert.Throws<UploadException>(() => messageWithNullObjectType.Validate());

            // Assert
            Assert.Equal(typeof(UploadException), ex.GetType());
            Assert.Equal(ValidationMessages.InvalidObjectType, ex.Message);
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)((UploadException)ex).StatusCode);
        }
    }
}

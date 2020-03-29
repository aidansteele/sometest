using System;
using UploadService.Models;
using UploadService.Predicates;
using UploadService.Tests.Unit.TestData;
using Xunit;

namespace UploadService.Tests.Unit
{
    public class MetadataPredicateTests
    {
        [Theory]
        [MemberData(nameof(TestDataForPredicates.ExceedsMaximumFileSizeData), MemberType = typeof(TestDataForPredicates))]
        public void TestExceedsMaximumFileSizePredicate(long value, bool expectedResult)
        {
            // Arrange
            var fmd = new FileMetadata() { Size = value };

            // Act
            var response = fmd.ExceedsMaximumFileSize();

            // Assert
            Assert.Equal(expectedResult, response);
        }

        [Theory]
        [MemberData(nameof(TestDataForPredicates.IsEmptyFileData), MemberType = typeof(TestDataForPredicates))]
        public void TestIsEmptyFilePredicate(long value, bool expectedResult)
        {
            // Arrange
            var fmd = new FileMetadata() { Size = value };

            // Act
            var response = fmd.IsEmptyFile();

            // Assert
            Assert.Equal(expectedResult, response);
        }

        [Theory]
        [MemberData(nameof(TestDataForPredicates.IsValidExtension), MemberType = typeof(TestDataForPredicates))]
        public void TestIsValidExtensionPredicate(string value, bool expectedResult)
        {
            // Arrange
            var fmd = new FileMetadata { Name = value };

            // Act
            var response = fmd.IsValidExtension();

            // Assert
            Assert.Equal(expectedResult, response);
        }

        [Theory]
        [MemberData(nameof(TestDataForPredicates.IsValidFilename), MemberType = typeof(TestDataForPredicates))]
        public void TestIsValidFilenamePredicate(string value, bool expectedResult)
        {
            // Arrange
            var fmd = new FileMetadata { Name = value };

            // Act
            var response = fmd.IsValidFilename();

            // Assert
            Assert.Equal(expectedResult, response);
        }

        [Theory]
        [MemberData(nameof(TestDataForPredicates.IsValidObjectType), MemberType = typeof(TestDataForPredicates))]
        public void TestIsValidObjectTypePredicate(string value, bool expectedResult)
        {
            // Arrange
            var fmd = new AssociationCreation { ObjectType = value };

            // Act
            var response = fmd.IsValidObjectType();

            // Assert
            Assert.Equal(expectedResult, response);
        }
    }
}

using System;
using System.Net;
using UploadService.Constants;
using UploadService.Models;
using UploadService.Models.Exceptions;
using UploadService.Validators;
using UploadService.Controllers;
using UploadService.Services;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace UploadService.Tests.Unit
{
    public class FileControllerTests
    {
        Mock<IUploadService> uploadService;
        CurrentUserContext userContext;

        public FileControllerTests(){
             uploadService  = new Mock<IUploadService>();
             userContext  = new CurrentUserContext(){UserId = Guid.NewGuid(), OrganisationId = Guid.NewGuid() };
        }

        [Fact]
        public async System.Threading.Tasks.Task PostV2_RequestWithMissingArgs_ReturnsBadRequest()
        {
            var controller = new FilesController(uploadService.Object, userContext);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var response = await controller.PostV2(string.Empty, string.Empty, string.Empty) as BadRequestObjectResult;
            Assert.Equal<int>(StatusCodes.Status400BadRequest, response.StatusCode.Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task PostV2_WithByteArrayBody_InvokesUploadServiceWithPopulatedMessage()
        {
            byte[] fileBytes={0xa,0x2,0xf};
            
            var fileId = Guid.NewGuid();
            var folderId = Guid.NewGuid();
            var userId = userContext.UserId;
            var orgId = userContext.OrganisationId;
            const string fileName = "filename.png";
            const string mimeType = "image/png";

            uploadService.Setup(x => x.UploadFile(It.IsAny<FileUploadMessage>()))
            .ReturnsAsync(new WriteResponse(){FileId=fileId});

            using(var memStream = new MemoryStream(fileBytes)) 
            {              
                var controller = new FilesController(uploadService.Object, userContext);
                controller.ControllerContext = new ControllerContext();
                var httpContext = new DefaultHttpContext();
                httpContext.Request.Body = memStream;
                httpContext.Request.ContentType = mimeType;
                controller.ControllerContext.HttpContext = httpContext;

                // Invoke the upload
                var response = await controller.PostV2(fileId.ToString(), folderId.ToString(), fileName) as CreatedResult;
                
                // Verify that UploadFile was called with a FileUploadMessage in expected state. 
                uploadService.Verify(x => x.UploadFile(
                    It.Is<FileUploadMessage>( f =>
                    f.Metadata.FileId == fileId &&
                    f.Metadata.Size == fileBytes.Length &&
                    f.Metadata.Name == fileName &&
                    f.Metadata.MimeType == mimeType &&
                    f.Metadata.UserId == userId &&
                    f.Metadata.OrganisationId == orgId &&
                    f.Metadata.ParentId == folderId
                    )), Times.Once);

                Assert.NotNull(response);
                Assert.Equal("/api/v2/files", response.Location);
                var writeResponse = response.Value as WriteResponse;
                Assert.Equal(fileId, writeResponse.FileId);
            }
        }
    }
}

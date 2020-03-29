using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UploadService.Models;
using UploadService.Services;
using UploadService.Converters;
using UploadService.Errors;
using System.Net.Http;
using System.IO;
using System;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;

namespace UploadService.Controllers
{
    public class FilesController : Controller
    {
        private readonly IUploadService _uploadService;
        private readonly CurrentUserContext _currentUserContext;

        public FilesController(IUploadService uploadService, CurrentUserContext currentUserContext)
        {
            _uploadService = uploadService;
            _currentUserContext = currentUserContext;
        }

        [Route("api/v1/[controller]")]      
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ReceivedFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(HttpContext.CreateErrorResponseModel("Invalid model state."));
            }

            if (file == null)
            {
                return BadRequest(HttpContext.CreateErrorResponseModel("Incoming file is null."));
            }

            var uploadMessage = FileUploadMessageBuilder.Init()
                .FromBody(file)
                .WithIdParameters(
                    _currentUserContext.UserId,
                    _currentUserContext.OrganisationId,
                    file.FolderId);

            var response = await _uploadService.UploadFile(uploadMessage);
            return Created("/api/v1/files", response);
        }

        [Route("api/v2/[controller]")]
        [HttpPost]
        public async Task<ActionResult> PostV2(
            [FromHeader] string fileId,
            [FromHeader] string folderId,
            [FromHeader] string fileName)
        {

        using (var mem = new MemoryStream())
        {
            Request.EnableBuffering();
            Request.Body.CopyTo(mem);
            mem.Position = 0;
            Guid fileIdGuid;
            Guid folderIdGuid;
            #region Request validation

            if(!Guid.TryParse(fileId, out fileIdGuid))
            {
                return BadRequest(HttpContext.CreateErrorResponseModel("Invalid model state. A guid fileId is Required"));
            }       
            if(!Guid.TryParse(folderId, out folderIdGuid))
            {
                return BadRequest(HttpContext.CreateErrorResponseModel("Invalid model state.. A guid folderId is Required"));
            }
            if(string.IsNullOrEmpty(fileName))
            {
                return BadRequest(HttpContext.CreateErrorResponseModel("Invalid model state.. fileName is Required"));
            }
            if (mem.Length == 0)
            {
                return BadRequest(HttpContext.CreateErrorResponseModel("Incoming file is empty."));
            }
            #endregion

            var fileUploadMessage = new FileUploadMessage {
                Content = mem,
                Metadata = new FileMetadata {
                    FileId = fileIdGuid,
                    Size = mem.Length,
                    Name = fileName,
                    MimeType = Request.ContentType,
                    UserId = _currentUserContext.UserId,
                    OrganisationId = _currentUserContext.OrganisationId,
                    ParentId = folderIdGuid
                }
            };

            var response = await _uploadService.UploadFile(fileUploadMessage);
            return Created("/api/v2/files", response);
        }
        }
    }
}

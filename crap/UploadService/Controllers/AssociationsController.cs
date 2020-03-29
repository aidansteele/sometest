using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UploadService.Errors;
using UploadService.Models;
using UploadService.Services;

namespace UploadService.Controllers
{
    [Route("api/v1/[controller]")]
    public class AssociationsController : Controller
    {
        private readonly CurrentUserContext _currentUserContext;
        private readonly IAssociationService _associateFileService;

        public AssociationsController(IAssociationService associateFileService, CurrentUserContext currentUserContext)
        {
            _currentUserContext = currentUserContext;
            _associateFileService = associateFileService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssociations([FromBody] AssociationCreation payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(HttpContext.CreateErrorResponseModel("Invalid model state."));
            }

            if (payload == null)
            {
                return BadRequest(HttpContext.CreateErrorResponseModel("Incoming payload is null."));
            }

            var response = await _associateFileService.AddAssociation(_currentUserContext.OrganisationId, payload);

            return Created("/api/v1/associations", response);
        }
    }
}

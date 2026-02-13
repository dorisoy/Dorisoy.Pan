using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dorisoy.Pan.API.Controllers.DocumentVersion
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentVersionController : BaseController
    {
        private readonly IMediator _mediator;

        public DocumentVersionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets the document versions.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentVersions(Guid id)
        {
            var updateUserProfilePhotoCommand = new GetDocumentVersionQuery()
            {
                Id = id
            };
            var result = await _mediator.Send(updateUserProfilePhotoCommand);
            return Ok(result);
        }

        /// <summary>
        /// Restores the document version.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="versionId">The version identifier.</param>
        /// <returns></returns>
        [HttpPost("{id}/restore/{versionId}")]
        public async Task<IActionResult> RestoreDocumentVersion(Guid id, Guid versionId)
        {
            var updateUserProfilePhotoCommand = new RestoreDocumentVersionCommand()
            {
                Id = versionId,
                DocumentId=id
            };
            var result = await _mediator.Send(updateUserProfilePhotoCommand);
            return ReturnFormattedResponse(result);
        }
    }
}

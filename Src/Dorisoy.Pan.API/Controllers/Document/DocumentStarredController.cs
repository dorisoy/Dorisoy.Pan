using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dorisoy.Pan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentStarredController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DocumentStarredController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Toggles the virtual folder starred.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> ToggleDocumentStarred(Guid id)
        {
            var toggleDocumentStarredCommand = new ToggleDocumentStarredCommand
            {
                Id = id
            };
            var result = await _mediator.Send(toggleDocumentStarredCommand);
            return Ok(result);
        }

        /// <summary>
        /// Gets the starred documents.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetStarredDocuments()
        {
            var getStarredDocumentsQuery = new GetStarredDocumentsQuery() { };
            var result = await _mediator.Send(getStarredDocumentsQuery);
            return Ok(result);
        }
    }
}

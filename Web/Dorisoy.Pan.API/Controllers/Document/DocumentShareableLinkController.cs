using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dorisoy.Pan.API.Controllers.Document
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentShareableLinkController : BaseController
    {
        private readonly IMediator _mediator;

        public DocumentShareableLinkController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates the shareable link.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateShareableLink(CreateDocumentShareableLinkCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Creates the shareable link.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentShareableLink(Guid id)
        {
            var query = new GetDocumentShareableLinkQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Deletes the document shareable link.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumentShareableLink(Guid id)
        {
            var query = new DeleteDocumentShareableLinkCommand { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }


    }
}

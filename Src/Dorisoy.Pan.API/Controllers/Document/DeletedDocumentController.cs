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
    public class DeletedDocumentController : BaseController
    {

        private readonly IMediator _mediator;

        public DeletedDocumentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets the deleted documents.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDeletedDocuments()
        {
            var getAllDeletedDocumentQuery = new GetAllDeletedDocumentQuery() { };
            var result = await _mediator.Send(getAllDeletedDocumentQuery);
            return Ok(result);
        }

        /// <summary>
        /// Restores the document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> RestoreDocument(Guid id)
        {
            var restoreDeletedFolderCommand = new RestoreDeletedDocumentCommand() { Id = id };
            var result = await _mediator.Send(restoreDeletedFolderCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            var restoreDeletedFolderCommand = new DeleteDeletedDocumentCommand() { Id = id };
            var result = await _mediator.Send(restoreDeletedFolderCommand);
            return ReturnFormattedResponse(result);
        }
    }
}

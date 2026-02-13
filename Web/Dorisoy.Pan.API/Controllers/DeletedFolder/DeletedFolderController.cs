using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Dorisoy.Pan.MediatR.Command;

namespace Dorisoy.Pan.API.Controllers.DeletedFolder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeletedFolderController : BaseController
    {

        private readonly IMediator _mediator;

        public DeletedFolderController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Gets the deleted folders.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDeletedFolders()
        {
            var getAllDeletedFolderQuery = new GetAllDeletedFolderQuery() { };
            var result = await _mediator.Send(getAllDeletedFolderQuery);
            return Ok(result);
        }

        /// <summary>
        /// Restores the folder.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> RestoreFolder(Guid id)
        {
            var restoreDeletedFolderCommand = new RestoreDeletedFolderCommand() { Id = id };
            var result = await _mediator.Send(restoreDeletedFolderCommand);
            return ReturnFormattedResponse(result);
        }
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllFolderAndDocument()
        {
            var deleteAllFolderAndFileCommand = new DeleteAllFolderAndFileCommand() {  };
            var result = await _mediator.Send(deleteAllFolderAndFileCommand);
            return Ok(result);
        }

        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolder(Guid id)
        {
            var deleteDeletedFolderCommand = new DeleteDeletedFolderCommand() { Id = id };
            var result = await _mediator.Send(deleteDeletedFolderCommand);
            return ReturnFormattedResponse(result);
        }
        
    }
}

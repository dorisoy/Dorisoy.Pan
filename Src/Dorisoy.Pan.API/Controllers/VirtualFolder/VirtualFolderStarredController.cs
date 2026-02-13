using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dorisoy.Pan.API.Controllers.VirtualFolder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VirtualFolderStarredController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VirtualFolderStarredController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Toggles the virtual folder starred.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> ToggleVirtualFolderStarred(Guid id)
        {
            var toggleVirtualFolderStarredCommand = new ToggleVirtualFolderStarredCommand
            {
                Id = id
            };
            var result = await _mediator.Send(toggleVirtualFolderStarredCommand);
            return Ok(result);
        }

        /// <summary>
        /// Gets the starred folders.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetStarredFolders()
        {
            var getStarredFoldersQuery = new GetStarredFoldersQuery() { };
            var result = await _mediator.Send(getStarredFoldersQuery);
            return Ok(result);
        }
    }
}

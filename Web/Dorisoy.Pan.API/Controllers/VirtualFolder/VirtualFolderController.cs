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
    public class VirtualFolderController : BaseController
    {
        private readonly IMediator _mediator;

        public VirtualFolderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("root")]
        public async Task<IActionResult> GetRootFolder()
        {
            var getActionQuery = new GetRootFolderQuery { };
            var result = await _mediator.Send(getActionQuery);
            return Ok(result);
        }

        /// <summary>
        /// Creates the folder.
        /// </summary>
        /// <param name="addFolderCommand">The add folder command.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateFolder(AddFolderCommand addFolderCommand)
        {
            var result = await _mediator.Send(addFolderCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Gets the root folder.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVirtualFolderById(Guid id)
        {
            var getActionQuery = new GetVirtualFoldersQuery
            {
                Id = id
            };
            var result = await _mediator.Send(getActionQuery);
            return Ok(result);
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetFolderDetailById(Guid id)
        {
            var getActionQuery = new GetVirtualFolderDetailByIdQuery
            {
                Id = id
            };
            var result = await _mediator.Send(getActionQuery);
            return Ok(result);
        }
        [HttpGet("{id}/source/{sourceId}")]
        public async Task<IActionResult> GetVirtualFolderForMoveAndCopy(Guid id, Guid sourceId)
        {
            var getActionQuery = new GetVirtualFolderForMoveAndCopyQuery
            {
                ParentId = id,
                SourceId = sourceId
            };
            var result = await _mediator.Send(getActionQuery);
            return Ok(result);
        }

        [HttpDelete("remove-right/folder/{folderId}/{physicalFolderId}/user/{userId}")]
        public async Task<IActionResult> RemoveFolderRightForUser(Guid folderId, Guid userId, Guid physicalFolderId)
        {
            var removeFolderRightForUserCommand = new RemoveFolderRightForUserCommand
            {
                FolderId = folderId,
                UserId = userId,
                PhysicalFolderId = physicalFolderId
            };
            var result = await _mediator.Send(removeFolderRightForUserCommand);
            return Ok(result);
        }

        [HttpPost("Folder/shared")]
        public async Task<IActionResult> SharedFolder([FromBody] SharedVirtualFolderCommand sharedVirtualFolderCommand)
        {
            var result = await _mediator.Send(sharedVirtualFolderCommand);
            return Ok(result);
        }
        [HttpPost("Document/shared")]
        public async Task<IActionResult> SharedDocument([FromBody] SharedDocumentCommand sharedDocumentCommand)
        {
            var result = await _mediator.Send(sharedDocumentCommand);
            return Ok(result);
        }
        [HttpGet("{id}/children")]
        public async Task<IActionResult> GetFolderChildsById(Guid id)
        {
            var getActionQuery = new GetFolderChildByIdQuery { Id = id };
            var result = await _mediator.Send(getActionQuery);
            return Ok(result);
        }
        [HttpGet("{id}/parentchild/shared")]
        public async Task<IActionResult> GetParentChildShared(Guid id)
        {
            var getActionQuery = new GetParentChildSharedQuery { Id = id };
            var result = await _mediator.Send(getActionQuery);
            return Ok(result);
        }

        [HttpGet("{id}/parent/shared")]
        public async Task<IActionResult> GetParentShared(Guid id)
        {
            var getActionQuery = new GetParentSharedQuery { Id = id };
            var result = await _mediator.Send(getActionQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get Folder Parents by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/parents")]
        public async Task<IActionResult> GetFolderParentsById(Guid id)
        {
            var getActionQuery = new GetFolderParentsByIdQuery { Id = id };
            var result = await _mediator.Send(getActionQuery);
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
            var deleteUserCommand = new DeleteVirtualFolderCommand { Id = id };
            var result = await _mediator.Send(deleteUserCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Searches the folder.
        /// </summary>
        /// <param name="searchString">The search string.</param>
        /// <returns></returns>
        [HttpGet("search/{searchString}")]
        public async Task<IActionResult> SearchFolder(string searchString)
        {
            var getActionQuery = new SearchVirtualFolderQuery { SearchString = searchString };
            var result = await _mediator.Send(getActionQuery);
            return Ok(result);
        }

        /// <summary>
        /// Gets the root folder.
        /// </summary>
        /// <returns></returns>
        [HttpGet("shared")]
        public async Task<IActionResult> GetSharedFolders()
        {
            var getActionQuery = new GetSharedFoldersQuery();
            var result = await _mediator.Send(getActionQuery);
            return Ok(result);
        }

        /// <summary>
        /// Renames the folder.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="renameFolderCommand">The rename folder command.</param>
        /// <returns></returns>
        [HttpPut("{id}/rename")]
        public async Task<IActionResult> RenameFolder(Guid id, [FromBody] RenameFolderCommand renameFolderCommand)
        {
            renameFolderCommand.Id = id;
            var result = await _mediator.Send(renameFolderCommand);
            return ReturnFormattedResponse(result);
        }
    }
}

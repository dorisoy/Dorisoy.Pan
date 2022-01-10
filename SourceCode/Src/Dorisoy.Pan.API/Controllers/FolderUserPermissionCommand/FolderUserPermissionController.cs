using Dorisoy.Pan.MediatR.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dorisoy.Pan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FolderUserPermissionController : BaseController
    {
        private readonly IMediator _mediator;
        public FolderUserPermissionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Give Folder User Permission
        /// </summary>
        /// <param name="addFolderUserPermissionCommand"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddFolderUserFolder(AddFolderUserPermissionCommand addFolderUserPermissionCommand)
        {
            var result = await _mediator.Send(addFolderUserPermissionCommand);
            return ReturnFormattedResponse(result);
        }
    }
}

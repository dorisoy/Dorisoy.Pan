using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dorisoy.Pan.API.Controllers.DocumentComment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentCommentController : BaseController
    {
        private readonly IMediator _mediator;

        public DocumentCommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets the document comments.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentComments(Guid id)
        {
            var updateUserProfilePhotoCommand = new GetAllDocumentCommentQuery()
            {
                Id = id
            };
            var result = await _mediator.Send(updateUserProfilePhotoCommand);
            return Ok(result);
        }

        /// <summary>
        /// Adds the document comment.
        /// </summary>
        /// <param name="addDocumentCommentCommand">The add document comment command.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddDocumentComment(AddDocumentCommentCommand addDocumentCommentCommand)
        {
            var result = await _mediator.Send(addDocumentCommentCommand);
            return ReturnFormattedResponse(result);
        }
    }
}

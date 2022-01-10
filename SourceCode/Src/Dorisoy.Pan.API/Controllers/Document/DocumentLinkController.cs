using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dorisoy.Pan.API.Controllers.Document
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentLinkController : BaseController
    {
        private readonly IMediator _mediator;

        public DocumentLinkController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Checks the link password.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        [HttpGet("{id}/check/{password}")]
        public async Task<IActionResult> CheckLinkPassword(Guid id, string password)
        {
            var query = new CheckDocumentLinkPasswordCommand
            {
                Id = id,
                Password = password
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Gets the document by link identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}/document")]
        public async Task<IActionResult> GetDocumentByLinkId(Guid id)
        {
            var query = new GetDocumenetByLinkIdQuery
            {
                Id = id
            };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Gets the link information by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        [HttpGet("info/{code}")]
        public async Task<IActionResult> GetLinkInfoByCode(string code)
        {
            var query = new GetLinkInfoByCodeQuery
            {
                Code = code
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

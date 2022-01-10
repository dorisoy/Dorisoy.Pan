using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dorisoy.Pan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedDocumentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SharedDocumentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets the shared document.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSharedDocument()
        {
            var getAllSharedDocumentsQuery = new GetAllSharedDocumentsQuery() { };
            var result = await _mediator.Send(getAllSharedDocumentsQuery);
            return Ok(result);
        }
    }
}

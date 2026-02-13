using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dorisoy.Pan.MediatR.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Dorisoy.Pan.API.Controllers.Email
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailController : BaseController
    {
        IMediator _mediator;
        public EmailController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Send mail.
        /// </summary>
        /// <param name="sendEmailCommand"></param>
        /// <returns></returns>
        [HttpPost(Name = "SendEmail")]
        [Produces("application/json", "application/xml", Type = typeof(void))]
        public async Task<IActionResult> SendEmail(SendEmailCommand sendEmailCommand)
        {
            var result = await _mediator.Send(sendEmailCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Sends the document.
        /// </summary>
        /// <param name="sendDocumentCommand">The send email command.</param>
        /// <returns></returns>
        [HttpPost("SendDocument")]
        [Produces("application/json", "application/xml", Type = typeof(void))]
        public async Task<IActionResult> SendDocument(SendDocumentCommand sendDocumentCommand)
        {
            var result = await _mediator.Send(sendDocumentCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Sends the folder.
        /// </summary>
        /// <param name="sendFolderCommad">The send email command.</param>
        /// <returns></returns>
        [HttpPost("SendFolder")]
        [Produces("application/json", "application/xml", Type = typeof(void))]
        public async Task<IActionResult> SendFolder(SendFolderCommad sendFolderCommad)
        {
            var result = await _mediator.Send(sendFolderCommad);
            return ReturnFormattedResponse(result);
        }
    }
}

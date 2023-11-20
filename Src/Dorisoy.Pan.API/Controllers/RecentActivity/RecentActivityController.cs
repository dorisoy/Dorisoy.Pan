using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dorisoy.Pan.API.Controllers.RecentActivity
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecentActivityController : BaseController
    {
        private readonly IMediator _mediator;
        public RecentActivityController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Recent Activity
        /// </summary>
        /// <param name="addRecentActivityCommand"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddRecentActivity(AddRecentActivityCommand addRecentActivityCommand)
        {
            var result = await _mediator.Send(addRecentActivityCommand);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRecentActivity()
        {
            var result = await _mediator.Send(new GetAllRecentActivityQuery());
            return Ok(result);
        }
    }
}

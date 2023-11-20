using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;

namespace Dorisoy.Pan.API.Controllers.Dashboard
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {

        public IMediator _mediator { get; set; }

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Active User Count
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetActiveUserCount")]
        [Produces("application/json", "application/xml", Type = typeof(int))]
        public async Task<IActionResult> GetActiveUserCount()
        {
            var getUserQuery = new GetActiveUserCountQuery { };
            var result = await _mediator.Send(getUserQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get Inactive User Count
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInactiveUserCount")]
        [Produces("application/json", "application/xml", Type = typeof(int))]
        public async Task<IActionResult> GetInactiveUserCount()
        {
            var getUserQuery = new GetInactiveUserCountQuery { };
            var result = await _mediator.Send(getUserQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get Total user count
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTotalUserCount")]
        [Produces("application/json", "application/xml", Type = typeof(int))]
        public async Task<IActionResult> GetTotalUserCount()
        {
            var getUserQuery = new GetTotalUserCountQuery { };
            var result = await _mediator.Send(getUserQuery);
            return Ok(result);
        }

        /// <summary>
        /// Gets the online users.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <returns></returns>
        [HttpGet("GetOnlineUsers")]
        [Produces("application/json", "application/xml", Type = typeof(List<UserDto>))]
        public async Task<IActionResult> GetOnlineUsers()
        {
            var query = new GetOnlineUsersQuery { };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

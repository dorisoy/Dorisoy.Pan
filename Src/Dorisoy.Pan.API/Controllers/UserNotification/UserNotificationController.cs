using Dorisoy.Pan.Data.Resources;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dorisoy.Pan.API.Controllers.UserNotification
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserNotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserNotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get New Notifications.
        /// </summary>
        /// <returns></returns>
        [HttpGet("new")]
        public async Task<IActionResult> GetNewNotifications()
        {
            var getUserNotificationsQuery = new GetNewNotificationsQuery { };
            var result = await _mediator.Send(getUserNotificationsQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All Notifications.
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllNotifications([FromQuery] NotificationSource notificationSource)
        {
            var getAllLoginAuditQuery = new GetAllNotificationQuery
            {
                NotificationSource = notificationSource
            };
            var result = await _mediator.Send(getAllLoginAuditQuery);
            var paginationMetadata = new
            {
                totalCount = result.TotalCount,
                pageSize = result.PageSize,
                skip = result.Skip,
                totalPages = result.TotalPages
            };
            Response.Headers.Append("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            return Ok(result);
        }

        /// <summary>
        /// Gets the user notification count.
        /// </summary>
        /// <returns></returns>
        [HttpGet("count")]
        public async Task<IActionResult> GetUserNotificationCount()
        {
            var getUserNotificationsQuery = new GetNotificationCountQuery { };
            var result = await _mediator.Send(getUserNotificationsQuery);
            return Ok(result);
        }

        /// <summary>
        /// Marks as read.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var markAsReadUserNotificationCommand = new MarkAsReadUserNotificationCommand { Id = id };
            await _mediator.Send(markAsReadUserNotificationCommand);
            return Ok();
        }
    }
}

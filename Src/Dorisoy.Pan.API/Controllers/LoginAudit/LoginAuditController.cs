using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Resources;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using Microsoft.AspNetCore.Http;

namespace Dorisoy.Pan.API.Controllers.LoginAudit
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoginAuditController : ControllerBase
    {
        public IMediator _mediator { get; set; }
        public LoginAuditController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get All Login Audit detail
        /// </summary>
        /// <param name="loginAuditResource"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", "application/xml", Type = typeof(LoginAuditList))]
        public async Task<IActionResult> GetLoginAudit([FromQuery] LoginAuditResource loginAuditResource)
        {
            var getAllLoginAuditQuery = new GetAllLoginAuditQuery
            {
                LoginAuditResource = loginAuditResource
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
    }
}

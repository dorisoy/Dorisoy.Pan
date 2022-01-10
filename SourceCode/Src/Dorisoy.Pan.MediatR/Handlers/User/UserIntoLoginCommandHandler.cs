using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class UserIntoLoginCommandHandler : IRequestHandler<UserIntoLoginCommand, ServiceResponse<UserAuthDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILoginAuditRepository _loginAuditRepository;
        private readonly IHubContext<UserHub, IHubClient> _hubContext;

        public UserIntoLoginCommandHandler(
            IUserRepository userRepository,
            UserManager<User> userManager,
            ILoginAuditRepository loginAuditRepository,
            IHubContext<UserHub, IHubClient> hubContext
            )
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _loginAuditRepository = loginAuditRepository;
            _hubContext = hubContext;
        }
        public async Task<ServiceResponse<UserAuthDto>> Handle(UserIntoLoginCommand request, CancellationToken cancellationToken)
        {
            var loginAudit = new LoginAuditDto
            {
                UserName = request.Email,
                RemoteIP = "",
                Status = LoginStatus.Error.ToString(),
                Latitude = "",
                Longitude = ""
            };

            var userInfo = await _userRepository
                     .All
                     .Where(c => c.UserName == request.Email)
                     .FirstOrDefaultAsync();
            if (userInfo !=null)
            {
              
                if (!userInfo.IsActive)
                {
                    await _loginAuditRepository.LoginAudit(loginAudit);
                    return ServiceResponse<UserAuthDto>.ReturnFailed(401, "UserName Or Password is InCorrect.");
                }
                loginAudit.Status = LoginStatus.Success.ToString();
                await _loginAuditRepository.LoginAudit(loginAudit);
                var claims = await _userManager.GetClaimsAsync(userInfo);
                var authUser = await _userRepository.BuildUserAuthObject(userInfo, claims);
                var onlineUser = new UserInfoToken
                {
                    Email = authUser.Email,
                    Id = authUser.Id
                };
                await _hubContext.Clients.All.Joined(onlineUser);
                return ServiceResponse<UserAuthDto>.ReturnResultWith200(authUser);
            }
            else
            {
                await _loginAuditRepository.LoginAudit(loginAudit);
                return ServiceResponse<UserAuthDto>.ReturnFailed(401, "UserName Or Password is InCorrect.");
            }
        }
    }
}

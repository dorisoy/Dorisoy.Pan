﻿using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Helper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, ServiceResponse<UserAuthDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILoginAuditRepository _loginAuditRepository;
        private readonly IHubContext<UserHub, IHubClient> _hubContext;
        private readonly IConnectionMappingRepository _connectionMappingRepository;

        public UserLoginCommandHandler(
            IUserRepository userRepository,
            SignInManager<User> signInManager,
            IConnectionMappingRepository connectionMappingRepository,
            UserManager<User> userManager,
            ILoginAuditRepository loginAuditRepository,
            IHubContext<UserHub, IHubClient> hubContext
            )
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _loginAuditRepository = loginAuditRepository;
            _hubContext = hubContext;
            _connectionMappingRepository = connectionMappingRepository;
        }
        public async Task<ServiceResponse<UserAuthDto>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var loginAudit = new LoginAuditDto
            {
                UserName = request.UserName,
                RemoteIP = request.RemoteIp,
                Status = LoginStatus.Error.ToString(),
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };
            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, false, false);
            if (result.Succeeded)
            {
                var userInfo = await _userRepository
                    .All
                    .Where(c => c.UserName == request.UserName)
                    .FirstOrDefaultAsync();
                if (!userInfo.IsActive)
                {
                    await _loginAuditRepository.LoginAudit(loginAudit);
                    return ServiceResponse<UserAuthDto>.ReturnFailed(401, "UserName Or Password is InCorrect.");
                }

                loginAudit.Status = LoginStatus.Success.ToString();

                await _loginAuditRepository.LoginAudit(loginAudit);
                var claims = await _userManager.GetClaimsAsync(userInfo);


                //用户身份认证
                var authUser = await _userRepository.BuildUserAuthObject(userInfo, claims);
                authUser.IP = request.RemoteIp;

                //var onlineUser = new UserInfoToken
                //{
                //    Email = authUser.Email,
                //    Id = authUser.Id,
                //    IP = request.RemoteIp
                //};

                //var allUsers = _connectionMappingRepository.GetAllUsersExceptThis(onlineUser).ToList();
                //var allUserIds = allUsers.Select(x => x.Id);

                //var users = await _userRepository.All.Where(c => allUserIds.Contains(c.Id))
                //    .Select(cs => new UserDto
                //    {
                //        Id = cs.Id,
                //        RaleName = cs.RaleName,
                //        Email = cs.Email,
                //        IP = request.RemoteIp,
                //        ProfilePhoto = ""
                //    }).ToListAsync();

                //if (users.Any())
                //{
                //    users.ForEach(x =>
                //    {
                //        x.IP = allUsers.Where(s => s.Id == x.Id).FirstOrDefault()?.IP ?? "";
                //    });
                //}

                //await _hubContext.Clients.All.OnlineUsers(users);

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

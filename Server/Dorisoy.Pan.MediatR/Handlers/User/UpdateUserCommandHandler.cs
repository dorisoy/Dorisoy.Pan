using AutoMapper;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Helper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ServiceResponse<UserDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        public UpdateUserCommandHandler(
            IMapper mapper,
            UserManager<User> userManager,
            UserInfoToken userInfoToken,
            ILogger<UpdateUserCommandHandler> logger
            )
        {
            _mapper = mapper;
            _userManager = userManager;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var appUser = await _userManager.FindByIdAsync(request.Id.ToString());
            var userClaims = await _userManager.GetClaimsAsync(appUser);
            if (userClaims.Count > 0)
                await _userManager.RemoveClaimsAsync(appUser, userClaims);
            if (appUser == null)
            {
                _logger.LogError("User does not exist.");
                return ServiceResponse<UserDto>.Return409("User does not exist.");
            }
            appUser.RaleName = request.RaleName;
            appUser.PhoneNumber = request.PhoneNumber;
            appUser.Address = request.Address;
            appUser.IsActive = request.IsActive;
            appUser.ModifiedDate = DateTime.UtcNow;
            appUser.ModifiedBy = _userInfoToken.Id;
            appUser.IsAdmin = request.IsAdmin;
            await _userManager.UpdateAsync(appUser);

            await AddUserClaim(appUser, request.UserClaims);
            return ServiceResponse<UserDto>.ReturnResultWith200(_mapper.Map<UserDto>(appUser));
        }

        private async Task AddUserClaim(User appUser, UserClaimDto userClaim)
        {
            try
            {
                await _userManager.AddClaimsAsync(appUser, new List<Claim>
                {
                    new Claim("IsFolderCreate",userClaim.IsFolderCreate ? "1":"0"),
                    new Claim("IsFileUpload",userClaim.IsFileUpload ? "1":"0"),
                    new Claim("IsDeleteFileFolder",userClaim.IsDeleteFileFolder ? "1":"0"),
                    new Claim("IsSharedFileFolder",userClaim.IsSharedFileFolder ? "1":"0"),
                    new Claim("IsSendEmail",userClaim.IsSendEmail ? "1":"0"),
                    new Claim("IsRenameFile",userClaim.IsRenameFile ? "1":"0"),
                    new Claim("IsDownloadFile",userClaim.IsDownloadFile ? "1":"0"),
                    new Claim("IsCopyFile",userClaim.IsCopyFile ? "1":"0"),
                    new Claim("IsCopyFolder",userClaim.IsCopyFolder ? "1":"0"),
                    new Claim("IsMoveFile",userClaim.IsMoveFile ? "1":"0"),
                    new Claim("IsSharedLink",userClaim.IsSharedLink ? "1":"0")
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

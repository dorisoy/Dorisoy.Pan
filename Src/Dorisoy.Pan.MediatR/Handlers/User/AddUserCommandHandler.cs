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
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, ServiceResponse<UserDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;
        private readonly ILogger<AddUserCommandHandler> _logger;
        private readonly PathHelper _pathHelper;
        public AddUserCommandHandler(
            IMapper mapper,
            UserManager<User> userManager,
            UserInfoToken userInfoToken,
            ILogger<AddUserCommandHandler> logger,
            PathHelper pathHelper
            )
        {
            _mapper = mapper;
            _userManager = userManager;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }
        public async Task<ServiceResponse<UserDto>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var appUser = await _userManager.FindByNameAsync(request.Email);
            if (appUser != null)
            {
                _logger.LogError("Email already exist for another user.");
                return ServiceResponse<UserDto>.Return409("Email already exist for another user.");
            }
            var entity = _mapper.Map<User>(request);
            entity.CreatedBy = _userInfoToken.Id;
            entity.ModifiedBy = _userInfoToken.Id;
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;
            entity.Id = Guid.NewGuid();
            entity.ProfilePhoto = _pathHelper.DefaultUserImage;
            IdentityResult result = await _userManager.CreateAsync(entity);
            if (!result.Succeeded)
            {
                return ServiceResponse<UserDto>.Return500();
            }
            AddUserClaim(entity, request.UserClaims);
            if (!string.IsNullOrEmpty(request.Password))
            {
                string code = await _userManager.GeneratePasswordResetTokenAsync(entity);
                IdentityResult passwordResult = await _userManager.ResetPasswordAsync(entity, code, request.Password);
                if (!passwordResult.Succeeded)
                {
                    return ServiceResponse<UserDto>.Return500();
                }
            }
            return ServiceResponse<UserDto>.ReturnResultWith200(_mapper.Map<UserDto>(entity));
        }
        private void AddUserClaim(User appUser, UserClaimDto userClaim)
        {
            _userManager.AddClaimsAsync(appUser, new List<Claim>
            {
                new Claim("IsFolderCreate",userClaim.IsFolderCreate.ToString()),
                new Claim("IsFileUpload",userClaim.IsFileUpload.ToString()),
                new Claim("IsDeleteFileFolder",userClaim.IsDeleteFileFolder.ToString()),
                new Claim("IsSharedFileFolder",userClaim.IsSharedFileFolder.ToString()),
                new Claim("IsSendEmail",userClaim.IsSendEmail.ToString()),
                new Claim("IsRenameFile",userClaim.IsRenameFile.ToString()),
                new Claim("IsDownloadFile",userClaim.IsDownloadFile.ToString()),
                new Claim("IsCopyFile",userClaim.IsCopyFile.ToString()),
                new Claim("IsCopyFolder",userClaim.IsCopyFolder.ToString()),
                new Claim("IsMoveFile",userClaim.IsMoveFile.ToString()),
                new Claim("IsSharedLink",userClaim.IsSharedLink.ToString())
            });
        }
    }
}

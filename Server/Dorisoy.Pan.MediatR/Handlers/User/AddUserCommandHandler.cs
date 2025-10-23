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
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, ServiceResponse<UserDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;
        private readonly ILogger<AddUserCommandHandler> _logger;
        private readonly PathHelper _pathHelper;
        private readonly IUserRepository _userRepository;


        public AddUserCommandHandler(
            IMapper mapper,
            UserManager<User> userManager,
            UserInfoToken userInfoToken,
            IUserRepository userRepository,
            ILogger<AddUserCommandHandler> logger,
            PathHelper pathHelper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<UserDto>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            try
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

                //AddUserClaim(entity, request.UserClaims);

                if (!string.IsNullOrEmpty(request.Password))
                {
                    //string code = await _userManager.GeneratePasswordResetTokenAsync(entity);
                    //IdentityResult passwordResult = await _userManager.ResetPasswordAsync(entity, code, request.Password);
                    //if (!passwordResult.Succeeded)
                    //{
                    //    return ServiceResponse<UserDto>.Return500();
                    //}
                    var ret = await _userRepository.ResetPasswordAsync(entity, request.Password);
                    if (!ret)
                    {
                        return ServiceResponse<UserDto>.Return500();
                    }

                }
                var dto = _mapper.Map<UserDto>(entity);

                return ServiceResponse<UserDto>.ReturnResultWith200(dto);
            }
            catch (Exception ex)
            {
                //  DbContext生命周期默认注入是Scope,每一次请求时创建一个实例，在当前请求的上下文中共用，当请求结束后，释放生命周期，释放数据库链接。若开启多线程，在不同的线程中使用同一个DbContext上下文，则报错如
                //{"A second operation was started on this context instance before a previous operation completed. This is usually caused by different threads concurrently using the same instance of DbContext. For more information on how to avoid threading issues with DbContext, see https://go.microsoft.com/fwlink/?linkid=2097913."}

                return ServiceResponse<UserDto>.Return500();
            }

  
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

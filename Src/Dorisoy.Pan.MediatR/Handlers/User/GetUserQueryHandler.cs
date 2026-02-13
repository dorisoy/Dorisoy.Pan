using AutoMapper;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Helper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Dorisoy.Pan.Data;
using System.Collections.Generic;
using System.Security.Claims;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ServiceResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserQueryHandler> _logger;
        private readonly UserManager<User> _userManager;
        public GetUserQueryHandler(
           IUserRepository userRepository,
            IMapper mapper,
            ILogger<GetUserQueryHandler> logger,
          UserManager<User> userManager
            )
        {

            _mapper = mapper;
            _userRepository = userRepository;
            _logger = logger;
            _userManager = userManager;
        }



        public async Task<ServiceResponse<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {

            var entity = await _userRepository.FindByInclude(c => c.Id == request.Id).FirstOrDefaultAsync();
            if (entity != null)
            {
                var claims = await _userManager.GetClaimsAsync(entity);
                var userDto = _mapper.Map<UserDto>(entity);
                userDto.UserClaims = getUserClaims(claims);
                return ServiceResponse<UserDto>.ReturnResultWith200(userDto);
            }
            else
            {
                _logger.LogError("User not found");
                return ServiceResponse<UserDto>.ReturnFailed(404, "User not found");
            }
        }
        private UserClaimDto getUserClaims(IList<Claim> lstClaims)
        {
            var userClaim = new UserClaimDto();
            foreach (var claim in lstClaims)
            {
                switch (claim.Type)
                {
                    case "IsFolderCreate":
                        userClaim.IsFolderCreate = claim.Value == "1";
                        break;
                    case "IsFileUpload":
                        userClaim.IsFileUpload = claim.Value == "1";
                        break;
                    case "IsDeleteFileFolder":
                        userClaim.IsDeleteFileFolder = claim.Value == "1";
                        break;
                    case "IsSharedFileFolder":
                        userClaim.IsSharedFileFolder = claim.Value == "1";
                        break;
                    case "IsSendEmail":
                        userClaim.IsSendEmail = claim.Value == "1";
                        break;
                    case "IsRenameFile":
                        userClaim.IsRenameFile = claim.Value == "1";
                        break;
                    case "IsDownloadFile":
                        userClaim.IsDownloadFile = claim.Value == "1";
                        break;
                    case "IsCopyFile":
                        userClaim.IsCopyFile = claim.Value == "1";
                        break;
                    case "IsCopyFolder":
                        userClaim.IsCopyFolder = claim.Value == "1";
                        break;
                    case "IsMoveFile":
                        userClaim.IsMoveFile = claim.Value == "1";
                        break;
                    case "IsSharedLink":
                        userClaim.IsSharedLink = claim.Value == "1";
                        break;
                }
            }
            return userClaim;
        }
    }
}

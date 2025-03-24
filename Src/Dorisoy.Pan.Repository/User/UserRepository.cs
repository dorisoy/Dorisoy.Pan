using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Resources;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.Repository
{
    public class UserRepository : GenericRepository<User, DocumentContext>,
          IUserRepository
    {
        private JwtSettings _settings = null;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly UserInfoToken _userInfo;
        private readonly PathHelper _pathHelper;

        public UserRepository(
            IUnitOfWork<DocumentContext> uow,
             JwtSettings settings,
             IPropertyMappingService propertyMappingService,
             UserInfoToken userInfo,
             PathHelper pathHelper) : base(uow)
        {
            _settings = settings;
            _propertyMappingService = propertyMappingService;
            _userInfo = userInfo;
            _pathHelper = pathHelper;
        }

        public async Task<UserList> GetUsers(UserResource userResource)
        {
            var collectionBeforePaging = All;
            collectionBeforePaging =
               collectionBeforePaging.ApplySort(userResource.OrderBy,
               _propertyMappingService.GetPropertyMapping<UserDto, User>());

            if (!string.IsNullOrWhiteSpace(userResource.FirstName))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(c =>
                   EF.Functions.Like(c.FirstName, $"%{userResource.FirstName}%") || EF.Functions.Like(c.LastName, $"%{userResource.FirstName}%"));
            }
            if (!string.IsNullOrWhiteSpace(userResource.LastName))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(c =>
                   EF.Functions.Like(c.LastName, $"%{userResource.LastName}%"));
            }
            if (!string.IsNullOrWhiteSpace(userResource.Email))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(c =>
                   EF.Functions.Like(c.Email, $"%{userResource.Email}%"));
            }
            if (!string.IsNullOrWhiteSpace(userResource.PhoneNumber))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(c =>
                   EF.Functions.Like(c.PhoneNumber, $"%{userResource.PhoneNumber}%"));
            }
            var isActive = userResource.IsActive == "0" ? false : true;
            collectionBeforePaging = collectionBeforePaging
                .Where(c => c.IsActive == isActive);

            var loginAudits = new UserList(_pathHelper);
            return await loginAudits.Create(
                collectionBeforePaging,
                userResource.Skip,
                userResource.PageSize
                );
        }

        public async Task<UserList> GetSharedUsers(UserResource userResource, List<Guid> folderUsers, List<Guid> documentUsers)
        {
            var collectionBeforePaging = All;
            collectionBeforePaging =
               collectionBeforePaging.ApplySort(userResource.OrderBy,
               _propertyMappingService.GetPropertyMapping<UserDto, User>());

            collectionBeforePaging = collectionBeforePaging.Where(c => c.Id != _userInfo.Id);

            if (!string.IsNullOrWhiteSpace(userResource.FirstName))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(c =>
                   EF.Functions.Like(c.FirstName, $"%{userResource.FirstName}%") || EF.Functions.Like(c.LastName, $"%{userResource.FirstName}%") || EF.Functions.Like(c.Email, $"%{userResource.FirstName}%"));
            }
            if (!string.IsNullOrWhiteSpace(userResource.LastName))
            {
                collectionBeforePaging = collectionBeforePaging
                    .Where(c =>
                   EF.Functions.Like(c.LastName, $"%{userResource.LastName}%"));
            }
            if (!string.IsNullOrWhiteSpace(userResource.Type))
            {
                if (userResource.Type.ToLower() == "folder")
                {
                    if (folderUsers.Count() > 0)
                    {
                        collectionBeforePaging = collectionBeforePaging.Where(c => !folderUsers.Any(x => x == c.Id));
                    }
                }
                else
                {
                    if (documentUsers.Count() > 0)
                    {
                        collectionBeforePaging = collectionBeforePaging.Where(c => !documentUsers.Any(x => x == c.Id));
                    }
                }
            }

            var isActive = userResource.IsActive == "0" ? false : true;
            collectionBeforePaging = collectionBeforePaging
                .Where(c => c.IsActive == isActive);

            var loginAudits = new UserList(_pathHelper);
            return await loginAudits.Create(
                collectionBeforePaging,
                userResource.Skip,
                userResource.PageSize
                );
        }

        public UserAuthDto BuildUserAuthObject(User appUser, IList<Claim> claims)
        {
            UserAuthDto ret = new UserAuthDto();
            // Set User Properties
            ret.Id = appUser.Id;
            ret.UserName = appUser.UserName;
            ret.FirstName = appUser.FirstName;
            ret.LastName = appUser.LastName;
            ret.Email = appUser.Email;
            ret.PhoneNumber = appUser.PhoneNumber;
            ret.IsAuthenticated = true;
            ret.ProfilePhoto = appUser.ProfilePhoto;
            ret.IsAdmin = appUser.IsAdmin;
            ret.Claims = getUserClaims(claims);
            ret.BearerToken = BuildJwtToken(ret, appUser.Id, claims);
            return ret;
        }

        private List<AppClaimDto> getUserClaims(IList<Claim> lstClaims)
        {
            var lstClaim = new List<AppClaimDto>();
            foreach (var claim in lstClaims)
            {
                switch (claim.Type)
                {
                    case "IsFolderCreate":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsFolderCreate", ClaimValue = claim.Value });
                        break;
                    case "IsFileUpload":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsFileUpload", ClaimValue = claim.Value });
                        break;
                    case "IsDeleteFileFolder":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsDeleteFileFolder", ClaimValue = claim.Value });
                        break;
                    case "IsSharedFileFolder":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsSharedFileFolder", ClaimValue = claim.Value });
                        break;
                    case "IsSendEmail":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsSendEmail", ClaimValue = claim.Value });
                        break;
                    case "IsRenameFile":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsRenameFile", ClaimValue = claim.Value });
                        break;
                    case "IsDownloadFile":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsDownloadFile", ClaimValue = claim.Value });
                        break;
                    case "IsCopyFile":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsCopyFile", ClaimValue = claim.Value });
                        break;
                    case "IsCopyFolder":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsCopyFolder", ClaimValue = claim.Value });
                        break;
                    case "IsMoveFile":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsMoveFile", ClaimValue = claim.Value });
                        break;
                    case "IsSharedLink":
                        lstClaim.Add(new AppClaimDto { ClaimType = "IsSharedLink", ClaimValue = claim.Value });
                        break;
                }
            }
            return lstClaim;
        }


        protected string BuildJwtToken(UserAuthDto authUser, Guid Id, IList<Claim> claims)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(_settings.Key));
            claims.Add(new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub.ToString(), Id.ToString()));
            claims.Add(new Claim("Email", authUser.Email));
            // Create the JwtSecurityToken object
            var token = new JwtSecurityToken(
              issuer: _settings.Issuer,
              audience: _settings.Audience,
              claims: claims,
              notBefore: DateTime.UtcNow,
              expires: DateTime.UtcNow.AddMinutes(
                  _settings.MinutesToExpiration),
              signingCredentials: new SigningCredentials(key,
                          SecurityAlgorithms.HmacSha256)
            );
            // Create a string representation of the Jwt token
            return new JwtSecurityTokenHandler().WriteToken(token); ;
        }
    }
}

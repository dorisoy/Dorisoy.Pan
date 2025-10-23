using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Dorisoy.Pan.Data.Dto
{
    public class UserAuthDto
    {
        public UserAuthDto()
        {
            BearerToken = string.Empty;
        }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string RaleName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BearerToken { get; set; }
        public bool IsAuthenticated { get; set; }
        public string ProfilePhoto { get; set; }
        public string IP { get; set; }
        public bool IsAdmin { get; set; }
        public List<AppClaimDto> Claims { get; set; }
    }
}

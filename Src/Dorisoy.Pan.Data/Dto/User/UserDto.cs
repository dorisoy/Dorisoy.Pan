using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ProfilePhoto { get; set; }
        public string Provider { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public long Size { get; set; }
        public UserClaimDto UserClaims { get; set; } = null;
    }
}

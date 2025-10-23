using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string RaleName { get; set; }
        public bool IsOwner { get; set; }
        public string ProfilePhoto { get; set; }
    }
}

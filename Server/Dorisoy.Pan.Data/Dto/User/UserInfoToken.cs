﻿using System;

namespace Dorisoy.Pan.Data.Dto
{
    public class UserInfoToken
    {
        public UserInfoToken() { }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string ConnectionId { get; set; }
        public string IP { get; set; }
    }
}

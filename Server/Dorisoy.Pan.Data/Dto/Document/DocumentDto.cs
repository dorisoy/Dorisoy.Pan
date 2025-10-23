﻿using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.Data.Dto
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? PhysicalFolderId { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string ThumbnailPath { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsStarred { get; set; }
        public UserInfoDto CreatedByUserInfo { get; set; }
        public List<UserInfoDto> Users { get; set; } = new List<UserInfoDto>();
        public List<UserInfoDto> PhysicalUsers { get; set; } = new List<UserInfoDto>();
        public List<Guid> DeletedUserIds { get; set; }
    }
}

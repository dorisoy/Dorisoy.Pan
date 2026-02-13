using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Dorisoy.Pan.Repository
{
    public class UserNotificationList : List<UserNotificationDto>
    {
        private readonly PathHelper _pathHelper;

        public UserNotificationList(PathHelper pathHelper)
        {
            _pathHelper = pathHelper;
        }

        public int Skip { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public UserNotificationList(List<UserNotificationDto> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public async Task<UserNotificationList> Create(IQueryable<UserNotification> source, int skip, int pageSize)
        {
            var count = await GetCount(source);
            var dtoList = await GetDtos(source, skip, pageSize);
            var dtoPageList = new UserNotificationList(dtoList, count, skip, pageSize);
            return dtoPageList;
        }

        public async Task<int> GetCount(IQueryable<UserNotification> source)
        {
            return await source.AsNoTracking().CountAsync();
        }

        public async Task<List<UserNotificationDto>> GetDtos(IQueryable<UserNotification> source, int skip, int pageSize)
        {
            var entities = await source
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
               .Select(c => new UserNotificationDto
               {
                   Id = c.Id,
                   CreatedDate = c.CreatedDate,
                   DocumentId = c.DocumentId,
                   DocumentName = c.DocumentId.HasValue ? c.Document.Name : "",
                   DocumentThumbnail = c.DocumentId.HasValue ? Path.Combine(_pathHelper.DocumentPath, c.Document.ThumbnailPath) : "",
                   FolderId = c.FolderId,
                   FolderName = c.FolderId.HasValue ? c.VirtualFolder.Name : "",
                   FromUserId = c.FromUserId,
                   Extension = c.DocumentId.HasValue ? c.Document.Extension : "",
                   IsRead = c.IsRead,
               }).ToListAsync();
            return entities;
        }
    }
}

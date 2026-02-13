using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.Repository
{
    public class UserList : List<UserDto>
    {
        private readonly PathHelper _pathHelper;
        public UserList(PathHelper pathHelper)
        {
            _pathHelper = pathHelper;
        }

        public int Skip { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public UserList(List<UserDto> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public async Task<UserList> Create(IQueryable<User> source, int skip, int pageSize)
        {
            var count = await GetCount(source);
            var dtoList = await GetDtos(source, skip, pageSize);
            var dtoPageList = new UserList(dtoList, count, skip, pageSize);
            return dtoPageList;
        }

        public async Task<int> GetCount(IQueryable<User> source)
        {
            return await source.AsNoTracking().CountAsync();
        }

        public async Task<List<UserDto>> GetDtos(IQueryable<User> source, int skip, int pageSize)
        {
            var entities = await source
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .Select(c => new UserDto
                {
                    Id = c.Id,
                    Email = c.Email,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    PhoneNumber = c.PhoneNumber,
                    IsActive = c.IsActive,
                    IsAdmin = c.IsAdmin,
                    ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath, c.ProfilePhoto)
                })
                .ToListAsync();

            entities.ForEach(e =>
            {
                e.Size = getSize(e.Id);
            });
            return entities;
        }

        public long getSize(Guid userId)
        {
            var path = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, userId.ToString());
            if (Directory.Exists(path))
            {
                DirectoryInfo dInfo = new DirectoryInfo(path);
                var size = DirectorySizeCalculation.DirectorySize(dInfo, true);
                return size;
            }
            else
                return 0;
        }
    }
}

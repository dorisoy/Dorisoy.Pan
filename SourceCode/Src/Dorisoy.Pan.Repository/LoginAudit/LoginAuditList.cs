using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;

namespace Dorisoy.Pan.Repository
{
    public class LoginAuditList : List<LoginAuditDto>
    {
        public LoginAuditList()
        {
        }

        public int Skip { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public LoginAuditList(List<LoginAuditDto> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public async Task<LoginAuditList> Create(IQueryable<LoginAudit> source, int skip, int pageSize)
        {
            var count = await GetCount(source);
            var dtoList = await GetDtos(source, skip, pageSize);
            var dtoPageList = new LoginAuditList(dtoList, count, skip, pageSize);
            return dtoPageList;
        }

        public async Task<int> GetCount(IQueryable<LoginAudit> source)
        {
            return await source.AsNoTracking().CountAsync();
        }

        public async Task<List<LoginAuditDto>> GetDtos(IQueryable<LoginAudit> source, int skip, int pageSize)
        {
            var entities = await source
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .Select(c => new LoginAuditDto
                {
                    Id = c.Id,
                    LoginTime = c.LoginTime,
                    Provider = c.Provider,
                    RemoteIP = c.RemoteIP,
                    Status = c.Status,
                    UserName = c.UserName,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                })
                .ToListAsync();
            return entities;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;

namespace Dorisoy.Pan.Repository
{
    public class NLogList : List<NLogDto>
    {
        public NLogList()
        {
        }

        public int Skip { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public NLogList(List<NLogDto> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public async Task<NLogList> Create(IQueryable<NLog> source, int skip, int pageSize)
        {
            var count = await GetCount(source);
            var dtoList = await GetDtos(source, skip, pageSize);
            var dtoPageList = new NLogList(dtoList, count, skip, pageSize);
            return dtoPageList;
        }

        public async Task<int> GetCount(IQueryable<NLog> source)
        {
            return await source.AsNoTracking().CountAsync();
        }

        public async Task<List<NLogDto>> GetDtos(IQueryable<NLog> source, int skip, int pageSize)
        {
            var entities = await source
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .Select(c => new NLogDto
                {
                    Id = c.Id,
                    Logged = c.Logged,
                    Level = c.Level,
                    Message = c.Message,
                    Logger = c.Logger,
                    Properties = c.Properties,
                    Callsite = c.Callsite,
                    Exception = c.Exception,
                    Source = c.Source
                })
                .ToListAsync();
            return entities;
        }
    }
}

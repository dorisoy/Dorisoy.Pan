using Dorisoy.Pan.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;

namespace Dorisoy.Pan.Common.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly ILogger<UnitOfWork<TContext>> _logger;
        private readonly UserInfoToken _userInfoToken;
        public UnitOfWork(
            TContext context,
            ILogger<UnitOfWork<TContext>> logger,
            UserInfoToken userInfoToken)
        {
            _context = context;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }
        public int Save()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    SetModifiedInformation();
                    var retValu = _context.SaveChanges();
                    transaction.Commit();
                    return retValu;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e, e.Message);
                    return 0;
                }
            }
        }
        public async Task<int> SaveAsync()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    SetModifiedInformation();
                    var val = await _context.SaveChangesAsync();
                    transaction.Commit();
                    return val;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e, e.Message);
                    return 0;
                }
            }
        }
        public TContext Context => _context;
        public void Dispose()
        {
            _context.Dispose();
        }

        private void SetModifiedInformation()
        {
            foreach (var entry in Context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = entry.Entity.CreatedDate == DateTime.MinValue.ToLocalTime() ? DateTime.UtcNow : entry.Entity.CreatedDate;
                    entry.Entity.CreatedBy = entry.Entity.CreatedBy != Guid.Empty ? entry.Entity.CreatedBy : _userInfoToken.Id;
                    entry.Entity.ModifiedBy = entry.Entity.ModifiedBy != Guid.Empty ? entry.Entity.ModifiedBy : _userInfoToken.Id;
                    entry.Entity.ModifiedDate = entry.Entity.ModifiedDate == DateTime.MinValue.ToLocalTime() ? DateTime.UtcNow : entry.Entity.ModifiedDate;
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity.IsDeleted)
                    {
                        entry.Entity.DeletedBy = _userInfoToken.Id;
                        entry.Entity.DeletedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        entry.Entity.ModifiedBy = _userInfoToken.Id;
                        entry.Entity.ModifiedDate = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}

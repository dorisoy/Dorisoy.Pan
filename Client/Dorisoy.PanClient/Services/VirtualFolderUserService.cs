using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Dorisoy.PanClient.Data.Contexts;

namespace Dorisoy.PanClient.Services;

public class VirtualFolderUserUserService : IVirtualFolderUserUserService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;
    private readonly IVirtualFolderService _virtualFolderService;

    public VirtualFolderUserUserService(IDbContextFactory<CaptureManagerContext> contextFactory, IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;
        _virtualFolderService = Locator.Current.GetService<IVirtualFolderService>();
    }


    public async Task<VirtualFolderUser> VirtualFolderPemission(Guid userid, Guid physicalFolderId)
    {
        return await Task.Run(async () =>
        {
            using (var context = _contextFactory.Create())
            {
                var virtualFolderPemission = await context.VirtualFolderUsers.IgnoreQueryFilters()
                          .Include(c => c.VirtualFolder)
                          .Where(c => c.UserId == userid
                              && c.VirtualFolder.PhysicalFolderId == physicalFolderId
                              && c.IsDeleted).FirstOrDefaultAsync();

                return virtualFolderPemission;
            }
        });
    }



    public async Task AddAsync(VirtualFolderUser virtualFolderUser)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                context.VirtualFolderUsers.Add(virtualFolderUser);
                context.SaveChanges();
            }
        });
    }

    public async Task AddRangeAsync(List<VirtualFolderUser> virtualFolderUsers)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                context.VirtualFolderUsers.AddRangeAsync(virtualFolderUsers);
                context.SaveChanges();
            }
        });
    }

    public async Task UpdateAsync(VirtualFolderUser virtualFolderUser)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.VirtualFolderUsers.First(e => e.Id == virtualFolderUser.Id);
                context.SaveChanges();
            }
        });
    }

    public async Task UpdateRangeAsync(List<VirtualFolderUser> virtualFolderUsers)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                context.VirtualFolderUsers.UpdateRange(virtualFolderUsers);
                context.SaveChanges();
            }
        });
    }

    public async Task DeleteAsync(VirtualFolderUser virtualFolderUser)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Set<VirtualFolderUser>().Find(virtualFolderUser.Id);
                if (entity != null)
                {
                    context.Entry(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }

    public async Task DeleteRangeAsync(List<VirtualFolderUser> virtualFolderUsers)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                //var entity = context.Set<VirtualFolderUser>().Find(virtualFolderUser.Id);
                //if (entity != null)
                //{
                //    context.Entry(entity).State = EntityState.Deleted;
                //    context.SaveChanges();
                //}
                context.VirtualFolderUsers.RemoveRange(virtualFolderUsers);
                context.SaveChanges();
            }
        });
    }

    public async void AssignPermission(Guid id, Guid folderId)
    {
        var user = Globals.CurrentUser;
        using (var context = _contextFactory.Create())
        {
            var data = context.VirtualFolderUsers
                .Where(c => c.FolderId == id)
                .Select(c => c.UserId)
                .ToList();

            if (data.Any())
            {
                var virtualFolderUsers = data.Select(c => new VirtualFolderUser
                {
                    FolderId = folderId,
                    UserId = c,
                    IsStarred = false,
                    IsDeleted = false,
                    ModifiedBy = user.Id,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedBy = user.Id,
                    CreatedDate = DateTime.UtcNow
                }).ToList();

                await AddRangeAsync(virtualFolderUsers);
            }
            else
            {
                await AddAsync(new VirtualFolderUser
                {
                    UserId = user.Id,
                    FolderId = folderId,
                    ModifiedBy = user.Id,
                    IsStarred = false,
                    IsDeleted = false,
                    ModifiedDate = DateTime.UtcNow,
                    CreatedBy = user.Id,
                    CreatedDate = DateTime.UtcNow,
                });
            }
        }
    }

    public async void AddFolderUsers(Guid folderId, List<Guid> users)
    {
        using (var context = _contextFactory.Create())
        {
            var lstFolderUsers = new List<VirtualFolderUser>();
            foreach (var userId in users)
            {
                var exits = context.VirtualFolderUsers.Any(c => c.FolderId == folderId && c.UserId == userId);
                if (!exits)
                {
                    lstFolderUsers.Add(new VirtualFolderUser
                    {
                        UserId = userId,
                        FolderId = folderId,
                        ModifiedBy = userId,
                        IsStarred = false,
                        IsDeleted = false,
                        ModifiedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                    });
                }
            }
            if (lstFolderUsers.Count() > 0)
            {
                await AddRangeAsync(lstFolderUsers);
            }
        }
    }

    public async Task AddVirtualFolderUsersChildsById(Guid id, List<Guid> users)
    {
        using (var context = _contextFactory.Create())
        {
            this.AddFolderUsers(id, users);

            var virtualChildFolders = await _virtualFolderService.GetChildsHierarchyById(id);

            if (virtualChildFolders.Count() > 0)
            {
                foreach (var virutalChildFolder in virtualChildFolders)
                {
                    this.AddFolderUsers(virutalChildFolder.Id, users);
                }
            }
        }
    }

    public async Task RemovedFolderUsers(List<HierarchyFolder> lstFolders, Guid userId)
    {
        using (var context = _contextFactory.Create())
        {
            var lstFolderUsers = new List<VirtualFolderUser>();
            foreach (var virtualFolder in lstFolders)
            {
                var virtualFolderUser = await context
                    .VirtualFolderUsers
                    .Where(c => c.FolderId == virtualFolder.Id && c.UserId == userId)
                    .FirstOrDefaultAsync();

                if (virtualFolderUser != null)
                {
                    lstFolderUsers.Add(virtualFolderUser);
                }
            }
            if (lstFolderUsers.Count > 0)
            {
                await DeleteRangeAsync(lstFolderUsers);
            }
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Dorisoy.PanClient.Data.Contexts;

namespace Dorisoy.PanClient.Services;

public class PhysicalFolderUserService : IPhysicalFolderUserService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;
    private readonly IPhysicalFolderService _physicalFolderService;

    public PhysicalFolderUserService(IDbContextFactory<CaptureManagerContext> contextFactory, IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;
        _physicalFolderService = Locator.Current.GetService<IPhysicalFolderService>();
    }



    public async Task AddAsync(PhysicalFolderUser physicalFolderUser)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                context.PhysicalFolderUsers.Add(physicalFolderUser);
                context.SaveChanges();
            }
        });
    }

    public async Task AddRangeAsync(List<PhysicalFolderUser> physicalFolderUsers)
    {
        using (var context = _contextFactory.Create())
        {
            await context.PhysicalFolderUsers.AddRangeAsync(physicalFolderUsers);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(PhysicalFolderUser physicalFolderUser)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                context.SaveChanges();
            }
        });
    }

    public async Task DeleteAsync(PhysicalFolderUser physicalFolderUser)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                if (physicalFolderUser != null)
                {
                    context.Entry(physicalFolderUser).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }

    public async Task DeleteRangeAsync(List<PhysicalFolderUser> physicalFolderUsers)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                context.PhysicalFolderUsers.RemoveRange(physicalFolderUsers);
                context.SaveChanges();
            }
        });
    }



    public async void AddFolderUsers(Guid id, List<Guid> users)
    {
        using (var context = _contextFactory.Create())
        {
            var lstPFUser = new List<PhysicalFolderUser>();
            foreach (var userId in users)
            {
                if (!context.PhysicalFolderUsers.Where(c => c.FolderId == id && c.UserId == userId).Any())
                {
                    lstPFUser.Add(new PhysicalFolderUser { FolderId = id, UserId = userId });
                }
            }
            if (lstPFUser.Count() > 0)
            {
                await AddRangeAsync(lstPFUser);
            }
        }
    }
    public async Task AddPhysicalFolderUsersChildsById(Guid id, List<Guid> users)
    {

        this.AddFolderUsers(id, users);
        var physicalChildFolders = await _physicalFolderService.GetChildsHierarchyById(id);
        if (physicalChildFolders.Count() > 0)
        {
            foreach (var physicalChildFolder in physicalChildFolders)
            {
                this.AddFolderUsers(physicalChildFolder.Id, users);
            }
        }
    }

    public async void AssignPermission(Guid id, Guid folderId)
    {
        var user = Globals.CurrentUser;
        using (var context = _contextFactory.Create())
        {
            var data = context.PhysicalFolderUsers
                .Where(c => c.FolderId == id)
                .Select(c => c.UserId)
                .ToList();

            if (data.Any())
            {
                var phycalFolderUsers = data.Select(c => new PhysicalFolderUser
                {
                    FolderId = folderId,
                    UserId = c
                }).ToList();

                await AddRangeAsync(phycalFolderUsers);
            }
            else
            {
                await AddAsync(new PhysicalFolderUser
                {
                    FolderId = folderId,
                    UserId = user.Id
                });
            }
        }
    }

    public async Task RemovedFolderUsers(List<HierarchyFolder> lstFolders, Guid userId)
    {
        using (var context = _contextFactory.Create())
        {
            var lstFolderUsers = new List<PhysicalFolderUser>();
            foreach (var folder in lstFolders)
            {
                var folderUser = await context.PhysicalFolderUsers
                    .Where(c => c.FolderId == folder.Id && c.UserId == userId)
                    .FirstOrDefaultAsync();

                if (folderUser != null)
                {
                    lstFolderUsers.Add(folderUser);
                }
            }
            if (lstFolderUsers.Count > 0)
            {
                await DeleteRangeAsync(lstFolderUsers);
            }
        }
    }

}

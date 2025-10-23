using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Dorisoy.Pan.Data.Contexts;
using Path = System.IO.Path;
namespace Dorisoy.Pan.Services;

public class PhysicalFolderService : IPhysicalFolderService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;
    private readonly SourceCache<PhysicalFolderModel, Guid> _items;

    public IObservable<IChangeSet<PhysicalFolderModel, Guid>> Connect() => _items.Connect();

    public PhysicalFolderService(IDbContextFactory<CaptureManagerContext> contextFactory, IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;

        _items = new SourceCache<PhysicalFolderModel, Guid>(e => e.Id);

        using (var context = _contextFactory.Create())
        {
            var physicalFolders = _mapper.ProjectTo<PhysicalFolderModel>(context.PhysicalFolders);
            _items.AddOrUpdate(physicalFolders);
        }
    }

    public async Task AddRangeAsync(List<PhysicalFolder> physicalFolders)
    {
        using (var context = _contextFactory.Create())
        {
            await context.PhysicalFolders.AddRangeAsync(physicalFolders);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<PhysicalFolderModel>> GetPhysicalFolders()
    {
        return await Task.Run(() =>
        {
            var physicalFolders = new List<PhysicalFolderModel>();
            using (var context = _contextFactory.Create())
            {
                var result = _mapper.ProjectTo<PhysicalFolderModel>(context.PhysicalFolders);
                physicalFolders = result.ToList();
            }
            return physicalFolders;
        });
    }

    public async Task<PhysicalFolderModel> ExistingPhysicalFolder(string name, Guid physicalFolderId, Guid userId)
    {
        using (var context = _contextFactory.Create())
        {
            var existingPhysicalFolder = await context.PhysicalFolders
            .Include(c => c.PhysicalFolderUsers)
            .FirstOrDefaultAsync(c => c.ParentId == physicalFolderId && c.Name == name && c.PhysicalFolderUsers.Any(c => c.UserId == userId));

            var result = _mapper.Map<PhysicalFolderModel>(existingPhysicalFolder);
            return result;
        }
    }

    public async Task<ServiceResult<PhysicalFolderModel>> AddAsync(PhysicalFolderModel model)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = new PhysicalFolder();
                _mapper.Map(model, entity);

                context.PhysicalFolders.Add(entity);
                context.SaveChanges();

                _mapper.Map(entity, model);

                _items.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }


    public async Task<ServiceResult<PhysicalFolder>> AddAsync(PhysicalFolder entity)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                context.PhysicalFolders.Add(entity);
                context.SaveChanges();
                //SqliteException: SQLite Error 19: 'UNIQUE constraint failed: PhysicalFolders.SystemFolderName
                return ServiceResult.Ok(entity);
            }
        });
    }



    public async Task<ServiceResult<PhysicalFolderModel>> UpdateAsync(PhysicalFolderModel model)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.PhysicalFolders.First(e => e.Id == model.Id);
                _mapper.Map(model, entity);

                context.SaveChanges();

                _mapper.Map(entity, model);

                _items.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }

    public async Task DeleteAsync(PhysicalFolderModel model)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Set<PhysicalFolder>().Find(model.Id);
                if (entity != null)
                {
                    context.Entry(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }

    public bool PhysicalFoldernameIsFree(Guid id, string physicalFoldername)
    {
        using (var context = _contextFactory.Create())
        {
            var count = context.PhysicalFolders.Count(e => e.Id == id && e.Name == physicalFoldername) > 1;
            return !count;
        }
    }

    public async Task<List<HierarchyFolder>> GetChildsHierarchyById(Guid id)
    {
        var idParam = new MySqlParameter("Id", id);

        //return await _uow.Context.HierarchyFolders
        //     .FromSqlRaw("CALL getPhysicalFolderChildsHierarchyById(@Id)", idParam)
        //     .ToListAsync();

        FormattableString sqlString = @$"WITH RECURSIVE
                              [tmp]([Id], [Name], [SystemFolderName],[PhysicalFolderId],[IsShared],[Level]) AS(
                                SELECT 
                                       [Id], 
                                       [Name], 
                                       SystemFolderName,
                                       upper(hex( randomblob(4)) || '-' || hex( randomblob(2))
                                     || '-' || '4' || substr( hex( randomblob(2)), 2) || '-'
                                     || substr('AB89', 1 + (abs(random()) % 4) , 1)  ||
                                     substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)))   as PhysicalFolderId,
                                       0 as IsShared,
                                       1 AS [Level]
                                FROM   [PhysicalFolders]
                                WHERE  [ParentId] = '{id.ToString().ToUpper()}'
                                UNION ALL
                                SELECT 
                                       [PhysicalFolders].[Id], 
                                       [tmp].[Name] || ', ' || [PhysicalFolders].[name] AS [Name], 
                                       [PhysicalFolders].SystemFolderName,
                                       upper(hex( randomblob(4)) || '-' || hex( randomblob(2))
                                     || '-' || '4' || substr( hex( randomblob(2)), 2) || '-'
                                     || substr('AB89', 1 + (abs(random()) % 4) , 1)  ||
                                     substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)))  as PhysicalFolderId,
                                       0 as IsShared,
                                       [tmp].[Level] + 1 AS [level]
                                FROM   [PhysicalFolders]
                                       JOIN [tmp] ON [PhysicalFolders].[ParentId] = [tmp].[Id]
                              )
  
                            SELECT * FROM  [tmp];";

        using (var context = _contextFactory.Create())
        {
            var folderHirarchy = await context.HierarchyFolders
                .FromSqlRaw("CALL getPhysicalFolderChildsHierarchyById(@Id)", idParam)
                .ToListAsync();
            return folderHirarchy.OrderByDescending(c => c.Level).ToList();
        }
    }


    /// <summary>
    /// 获取层次结构文件夹(CTE集合运算)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<List<HierarchyFolder>> GetParentsHierarchyById(Guid id)
    {

        var idParam = new MySqlParameter("Id", id);
        //var folderHirarchy = await _uow.Context.HierarchyFolders
        //     .FromSqlRaw("CALL getPhysicalFolderParentsHierarchyById(@Id)", idParam)
        //     .ToListAsync();
        //return folderHirarchy.OrderByDescending(c => c.Level).ToList();

        FormattableString sqlString = @$"WITH RECURSIVE
                              [tmp]([Id], [Name], [SystemFolderName],[PhysicalFolderId],[ParentId],[IsShared],[Level]) AS(
                                SELECT 
                                       [Id], 
                                       [Name], 
                                       SystemFolderName,
                                        ParentId,
                                       upper(hex( randomblob(4)) || '-' || hex( randomblob(2))
                                     || '-' || '4' || substr( hex( randomblob(2)), 2) || '-'
                                     || substr('AB89', 1 + (abs(random()) % 4) , 1)  ||
                                     substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)))   as PhysicalFolderId,
                                       0 as IsShared,
                                       1 AS [Level]
                                FROM   [PhysicalFolders]
                                WHERE  [ParentId] = '{id.ToString().ToUpper()}'
                                UNION ALL
                                SELECT 
                                       [PhysicalFolders].[Id], 
                                       [tmp].[Name] || ', ' || [PhysicalFolders].[name] AS [Name], 
                                       [PhysicalFolders].SystemFolderName,
                                       [PhysicalFolders].ParentId as ParentId,
                                       upper(hex( randomblob(4)) || '-' || hex( randomblob(2))
                                     || '-' || '4' || substr( hex( randomblob(2)), 2) || '-'
                                     || substr('AB89', 1 + (abs(random()) % 4) , 1)  ||
                                     substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)))  as PhysicalFolderId,
                                       0 as IsShared,
                                       [tmp].[Level] + 1 AS [level]
                                FROM   [PhysicalFolders]
                                       JOIN [tmp] ON [PhysicalFolders].[ParentId] = [tmp].[Id]
                              )
  
                            SELECT * FROM  [tmp];";

        using (var context = _contextFactory.Create())
        {
            //FromSql'操作的结果中不存在所需的列'ParentId

            var folderHirarchy = await context.HierarchyFolders
                .FromSqlRaw("CALL getPhysicalFolderParentsHierarchyById(@Id)", idParam)
                .ToListAsync();
            return folderHirarchy.OrderByDescending(c => c.Level).ToList();
        }
    }

    /// <summary>
    /// 获取父级别文件夹路径
    /// </summary>
    /// <param name="childId"></param>
    /// <returns></returns>
    public async Task<string> GetParentFolderPath(Guid childId)
    {
        using (var context = _contextFactory.Create())
        {
            var parents = await GetParentsHierarchyById(childId);
            return string.Join(Path.DirectorySeparatorChar, parents.Select(c => c.Name));
        }
    }

    /// <summary>
    /// 获取父级原始文件夹路径
    /// </summary>
    /// <param name="childId"></param>
    /// <returns></returns>
    public async Task<string> GetParentOriginalFolderPath(Guid childId)
    {
        using (var context = _contextFactory.Create())
        {
            var parents = await GetParentsHierarchyById(childId);
            return string.Join(Path.DirectorySeparatorChar, parents.Select(c => c.Name));
        }
    }

    /// <summary>
    /// 添加文件夹
    /// </summary>
    /// <param name="name"></param>
    /// <param name="virtualParentId"></param>
    /// <param name="physicalFolderId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ServiceResult<VirtualFolderInfoModel>> AddFolder(string name, Guid virtualParentId, Guid? physicalFolderId, Guid userId)
    {
        try
        {
            var vfs = Locator.Current.GetRequiredService<IVirtualFolderService>();
            var vfus = Locator.Current.GetRequiredService<IVirtualFolderUserUserService>();
            var pfus = Locator.Current.GetRequiredService<IPhysicalFolderUserService>();

            using (var context = _contextFactory.Create())
            {
                var existingEntityPermission = await context.VirtualFolderUsers
                  .IgnoreQueryFilters()
                  .Where(c => c.VirtualFolder.Name == name
                      && c.VirtualFolder.ParentId == virtualParentId
                      && c.UserId == userId && c.IsDeleted)
                  .FirstOrDefaultAsync();

                // 存在权限时
                if (existingEntityPermission != null)
                {
                    existingEntityPermission.IsDeleted = false;
                    await vfus.UpdateAsync(existingEntityPermission);
                    var virtualFolderInfoModel = await GetVirtualFolderInfoDto(existingEntityPermission.FolderId, true);
                    return ServiceResult.Ok(virtualFolderInfoModel);
                }

                // 存在文件夹时
                var existingEntity = await context.PhysicalFolders.Where(c => c.Name == name
                && c.ParentId == physicalFolderId && c.PhysicalFolderUsers.Any(phu => phu.UserId == userId))
                   .FirstOrDefaultAsync();
                if (existingEntity != null)
                {
                    return ServiceResult.Fail<VirtualFolderInfoModel>("文件夹已经存在");
                }

                // 物理文件夹
                var _physicalFolderId = Guid.NewGuid();
                var folder = new PhysicalFolder
                {
                    Id = _physicalFolderId,
                    Name = name,
                    ParentId = physicalFolderId,
                    Size = "",
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = userId,
                    ModifiedDate = DateTime.Now,
                    IsDeleted = false
                };
                await AddAsync(folder);

                //更新权限
                pfus.AssignPermission(physicalFolderId.Value, _physicalFolderId);

                var virtualFolderReturn = new VirtualFolder();
                var parentVirtualFolders = await vfs.GetVirualFoldersByPhysicalId(physicalFolderId.Value);
                foreach (var parentVirtualFolder in parentVirtualFolders)
                {
                    var virtualFolder = new VirtualFolder()
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        ParentId = parentVirtualFolder.Id,
                        PhysicalFolderId = _physicalFolderId,
                        Size = "",
                        IsShared = false,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = userId,
                        ModifiedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    await vfs.AddAsync(virtualFolder);

                    //更新权限
                    vfus.AssignPermission(virtualParentId, virtualFolder.Id);
                    if (parentVirtualFolder.Id == virtualParentId)
                    {
                        virtualFolderReturn = virtualFolder;
                    }
                }

                var _pathHelper = new PathHelper();
                // 创建目录
                var folderPath = await GetParentFolderPath(physicalFolderId.Value);
                var fullFolderPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, userId.ToString());

                //FromSql'操作的结果中不存在所需的列'ParentId

                //创建全路径文件夹
                if (!Directory.Exists(fullFolderPath))
                    Directory.CreateDirectory(fullFolderPath);

                var vf = await GetVirtualFolderInfoDto(virtualFolderReturn.Id);

                return ServiceResult.Ok(vf);
            }
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail<VirtualFolderInfoModel>(ex.Message);
        }
    }


    private async Task<VirtualFolderInfoModel> GetVirtualFolderInfoDto(Guid id, bool isRestore = false)
    {
        using (var context = _contextFactory.Create())
        {
            var virtualFolderInfo = await context.VirtualFolders
                .Include(c => c.PhysicalFolder)
            .ThenInclude(c => c.PhysicalFolderUsers)
            .ThenInclude(c => c.User)
            .Where(c => c.Id == id)
            .Select(c => new VirtualFolderInfoModel
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId,
                PhysicalFolderId = c.PhysicalFolderId,
                IsRestore = isRestore,
                IsShared = c.IsShared,
                Users = c.PhysicalFolder.PhysicalFolderUsers.Select(cs => new UserModel
                {
                    Email = cs.User.Email,
                    RaleName = cs.User.RaleName,
                    Id = cs.UserId,
                }).ToList()
            }).FirstOrDefaultAsync();

            return virtualFolderInfo;
        }
    }
}

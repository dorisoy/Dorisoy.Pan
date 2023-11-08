using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Dorisoy.PanClient.Common;
using Dorisoy.PanClient.Data.Contexts;
using Dorisoy.PanClient.Utils;

namespace Dorisoy.PanClient.Services;

public class VirtualFolderService : IVirtualFolderService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;

    private readonly SourceCache<VirtualFolderModel, Guid> _items;
    public IObservable<IChangeSet<VirtualFolderModel, Guid>> Connect() => _items.Connect();


    private readonly SourceCache<VirtualFolderInfoModel, Guid> _folders;
    public IObservable<IChangeSet<VirtualFolderInfoModel, Guid>> FolderConnect() => _folders.Connect();


    public VirtualFolderService(IDbContextFactory<CaptureManagerContext> contextFactory, IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;

        _items = new SourceCache<VirtualFolderModel, Guid>(e => e.Id);
        _folders = new SourceCache<VirtualFolderInfoModel, Guid>(e => e.Id);


        using (var context = _contextFactory.Create())
        {
            var vrtualFolders = _mapper.ProjectTo<VirtualFolderModel>(context.VirtualFolders);
            _items.AddOrUpdate(vrtualFolders);
        }
    }

    public async Task AddRangeAsync(List<VirtualFolder> virtualFolders)
    {
        using (var context = _contextFactory.Create())
        {
            await context.VirtualFolders.AddRangeAsync(virtualFolders);
            await context.SaveChangesAsync();
        }
    }


    public async Task<List<VirtualFolder>> VirtualFoldersToCreate(Guid physicalFolderId)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var virtualFoldersToCreate = context.VirtualFolders
                .Where(c => c.PhysicalFolderId == physicalFolderId)
                .ToList();
                return virtualFoldersToCreate;
            }
        });
    }


    public async Task<List<VirtualFolderModel>> GetVirtualFolders()
    {
        return await Task.Run(() =>
        {
            var vrtualFolders = new List<VirtualFolderModel>();
            using (var context = _contextFactory.Create())
            {
                var result = _mapper.ProjectTo<VirtualFolderModel>(context.VirtualFolders);
                vrtualFolders = result.ToList();
            }
            return vrtualFolders;
        });
    }

    public async Task<ServiceResult<VirtualFolder>> AddAsync(VirtualFolder entity)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                context.VirtualFolders.Add(entity);
                context.SaveChanges();

                return ServiceResult.Ok(entity);
            }
        });
    }

    public async Task<ServiceResult<VirtualFolderModel>> AddAsync(VirtualFolderModel model)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = new VirtualFolder();
                _mapper.Map(model, entity);

                context.VirtualFolders.Add(entity);
                context.SaveChanges();

                _mapper.Map(entity, model);

                _items.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }

    public async Task<ServiceResult<VirtualFolderModel>> UpdateAsync(VirtualFolderModel model)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.VirtualFolders.First(e => e.Id == model.Id);
                _mapper.Map(model, entity);

                context.SaveChanges();

                _mapper.Map(entity, model);

                _items.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }

    public async Task DeleteAsync(VirtualFolderModel model)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Set<VirtualFolder>().Find(model.Id);
                if (entity != null)
                {
                    context.Entry(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }

    public bool VirtualFoldernameIsFree(Guid id, string vrtualFoldername)
    {
        using (var context = _contextFactory.Create())
        {
            var count = context.VirtualFolders.Count(e => e.Id == id && e.Name == vrtualFoldername) > 1;
            return !count;
        }
    }

    public async Task<VirtualFolder> GetVirtualFolder(string name, Guid? parentId = null)
    {
        return await Task.Run(async () =>
        {
            using (var context = _contextFactory.Create())
            {
                if (parentId.HasValue)
                {
                    return await context.VirtualFolders.Where(c => c.Name == name && c.ParentId == parentId.Value).FirstOrDefaultAsync();
                }
                else
                {
                    return await context.VirtualFolders.Where(c => c.Name == name)
                   .FirstOrDefaultAsync();
                }

            }
        });
    }



    /// <summary>
    /// 获取虚拟根目录
    /// </summary>
    /// <returns></returns>
    public async Task<VirtualFolder> GetRootFolder()
    {
        return await Task.Run(async () =>
        {
            using (var context = _contextFactory.Create())
            {
                return await context.VirtualFolders
                .Where(c => c.ParentId == null)
                .FirstOrDefaultAsync();
            }
        });
    }

    /// <summary>
    /// 根据物理目录获取虚拟根目录
    /// </summary>
    /// <param name="PhysicalFolderId"></param>
    /// <returns></returns>
    public async Task<List<VirtualFolder>> GetVirualFoldersByPhysicalId(Guid PhysicalFolderId)
    {
        using (var context = _contextFactory.Create())
        {
            return await context.VirtualFolders.Where(c => c.PhysicalFolderId == PhysicalFolderId).ToListAsync();
        }
    }

    /// <summary>
    /// 获取虚拟目录
    /// </summary>
    /// <param name="folderId"></param>
    /// <returns></returns>
    public async Task<VirtualFolder> FindAsync(Guid folderId)
    {
        using (var context = _contextFactory.Create())
        {
            return await context.VirtualFolders
                .Where(c => c.Id == folderId)
                .FirstOrDefaultAsync();
        }
    }


    /// <summary>
    /// 根据物理目录获取用户目录
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<List<Guid>> GetFolderUserByPhysicalFolderId(Guid id)
    {
        using (var context = _contextFactory.Create())
        {
            var entities = await context.VirtualFolders.Include(c => c.VirtualFolderUsers)
              .Where(c => c.PhysicalFolderId == id)
              .Select(c => c.VirtualFolderUsers.Select(cs => cs.UserId))
              .ToListAsync();
            return entities.SelectMany(c => c).ToList();
        }
    }


    /// <summary>
    /// 递归获取文件夹名称
    /// </summary>
    /// <param name="name"></param>
    /// <param name="Id"></param>
    /// <param name="index"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<string> GetFolderName(string name, Guid Id, int index, Guid userId)
    {
        var modifiedName = "";

        if (index != 0)
        {
            modifiedName = name + "(" + index + ")";
        }
        else
        {
            modifiedName = name;
        }

        using (var context = _contextFactory.Create())
        {
            var distinationSameNameFolder = await context.VirtualFolders
                .Where(c => c.ParentId == Id
                && c.Name == modifiedName
                && c.VirtualFolderUsers.Any(c => c.UserId == userId))
                .FirstOrDefaultAsync();

            if (distinationSameNameFolder != null)
            {
                return await GetFolderName(name, distinationSameNameFolder.ParentId.Value, ++index, userId);
            }
            else
            {
                return modifiedName;
            }
        }
    }


    /// <summary>
    /// 获取层次结构文件夹(CTE集合运算)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<List<HierarchyFolder>> GetChildsHierarchyById(Guid id)
    {
        var idParam = new MySqlParameter("Id", id);

        //return await _uow.Context.HierarchyFolders
        //     .FromSqlRaw("CALL getVirtualFolderChildsHierarchyById(@Id)", idParam)
        //     .ToListAsync();

        //SQLLIte
        FormattableString sqlString = @$"WITH RECURSIVE
                                              [tmp]([Id], [Name], [SystemFolderName],[ParentId],[PhysicalFolderId],[IsShared],[Level]) AS(
                                                SELECT 
                                                       [Id], 
                                                       [Name], 
                                                       0 as SystemFolderName,
                                                       ParentId,
                                                       PhysicalFolderId,
                                                       0 as IsShared,
                                                       0 AS [Level]
                                                FROM   [VirtualFolders]
                                                WHERE  [ParentId] = '{id.ToString().ToUpper()}'
                                                UNION ALL
                                                SELECT 
                                                       [VirtualFolders].[Id], 
                                                       [VirtualFolders].[name] AS [Name], 
                                                       0 as SystemFolderName,
                                                       [VirtualFolders].ParentId as ParentId,
                                                       [VirtualFolders].PhysicalFolderId as PhysicalFolderId,
                                                       0 as IsShared,
                                                       [tmp].[Level] + 1 AS [level]
                                                FROM   [VirtualFolders]
                                                       JOIN [tmp] ON [VirtualFolders].[ParentId] = [tmp].[Id]
                                              )
                                            SELECT * FROM  [tmp];";

        using (var context = _contextFactory.Create())
        {
            var folderHirarchy = await context.HierarchyFolders
                .FromSqlRaw("CALL getSharedChildsHierarchyById(@Id)", idParam)
                .ToListAsync();
            return folderHirarchy.ToList();
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
        //     .FromSqlRaw("CALL getVirtualFolderParentsHierarchyById(@Id)", idParam)
        //     .ToListAsync();
        //return folderHirarchy.OrderByDescending(c => c.Level).ToList();

        FormattableString sqlString = @$"WITH RECURSIVE
                                              [tmp]([Id], [Name], [SystemFolderName],[ParentId],[PhysicalFolderId],[IsShared],[Level]) AS(
                                                SELECT 
                                                       [Id], 
                                                       [Name], 
                                                       0 as SystemFolderName,
                                                       ParentId,
                                                       PhysicalFolderId,
                                                       0 as IsShared,
                                                       0 AS [Level]
                                                FROM   [VirtualFolders]
                                                WHERE  [Id] = '{id.ToString().ToUpper()}'
                                                UNION ALL
                                                SELECT 
                                                       [VirtualFolders].[Id], 
                                                       [VirtualFolders].[name] AS [Name], 
                                                       0 as SystemFolderName,
                                                       [VirtualFolders].ParentId as ParentId,
                                                       [VirtualFolders].PhysicalFolderId as PhysicalFolderId,
                                                       0 as IsShared,
                                                       [tmp].[Level] + 1 AS [level]
                                                FROM   [VirtualFolders]
                                                       JOIN [tmp] ON [VirtualFolders].[Id] = [tmp].[ParentId]
                                              )
                                            SELECT * FROM  [tmp];";

        using (var context = _contextFactory.Create())
        {
            var folderHirarchy = await context.HierarchyFolders
              .FromSqlRaw("CALL getBreadsParentsHierarchyById(@Id)", idParam)
                .ToListAsync();
            return folderHirarchy.OrderByDescending(c => c.Level).ToList();
        }
    }

    /// <summary>
    /// 获取父级文件夹路径
    /// </summary>
    /// <param name="childId"></param>
    /// <returns></returns>
    public async Task<string> GetParentFolderPath(Guid childId)
    {
        var parents = await GetParentsHierarchyById(childId);
        return string.Join("/", parents.Select(c => c.Name));
    }

    public async Task<List<Guid>> GetFolderUserIds(string folderName, Guid parentPhysicalFolderId, Guid parentVirtualFolderId, Guid userId)
    {
        using (var context = _contextFactory.Create())
        {
            var virtualParentFolder = await context.VirtualFolders.FindAsync(parentVirtualFolderId);
            if (virtualParentFolder.ParentId == null) // Root Folder
            {
                var existingEntityPermission = await context.VirtualFolders
                    .IgnoreQueryFilters()
                    .Include(c => c.VirtualFolderUsers)
                    .Where(c => c.Name == folderName
                    && c.ParentId == parentVirtualFolderId
                    && c.VirtualFolderUsers.Any(c => c.UserId == userId))
                    .FirstOrDefaultAsync();
                if (existingEntityPermission != null)
                {
                    return existingEntityPermission.VirtualFolderUsers.Select(c => c.UserId).ToList();
                }
                return new List<Guid>();
            }

            return await context.VirtualFolders
                .Where(c => c.PhysicalFolderId == parentPhysicalFolderId)
                .IgnoreQueryFilters()
                .SelectMany(c => c.VirtualFolderUsers)
                .Select(c => c.UserId).ToListAsync();
        }
    }

    public async Task<List<VirtualFolderInfoModel>> GetVirtualFolderInfos(List<Guid> folderIdsToReturn)
    {
        using (var context = _contextFactory.Create())
        {
            var virtualFolderInfo = await context.VirtualFolders
            .Include(c => c.PhysicalFolder)
            .ThenInclude(c => c.PhysicalFolderUsers)
            .ThenInclude(c => c.User)
            .Where(c => folderIdsToReturn.Contains(c.Id))
            .Select(c => new VirtualFolderInfoModel
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId,
                PhysicalFolderId = c.PhysicalFolderId,
                IsShared = c.IsShared,
                Users = c.PhysicalFolder.PhysicalFolderUsers.Select(cs => new UserModel
                {
                    Email = cs.User.Email,
                    RaleName = cs.User.RaleName,
                    Id = cs.UserId
                }).ToList()
            }).ToListAsync();

            return virtualFolderInfo;
        }
    }


    /// <summary>
    /// 获取指定用户父目录的虚拟目录
    /// </summary>
    /// <param name="folderIdsToReturn"></param>
    /// <returns></returns>
    public async Task<List<VirtualFolderInfoModel>> GetVirtualFolders(Guid parentId, Guid userid)
    {
        var _pathHelper = new PathHelper();
        using (var context = _contextFactory.Create())
        {
            //转换此查询需要SQL APPLY操作，而SQLite不支持此操作。
            //Translating this query requires the SQL APPLY operation, which is not supported on SQLite.”

            var entities = new List<VirtualFolderInfoModel>();

            var vfs = await context.VirtualFolders
             .Include(c => c.PhysicalFolder)
             .ThenInclude(c => c.PhysicalFolderUsers)
             .ThenInclude(c => c.User)
             .Where(c => c.ParentId == parentId && c.VirtualFolderUsers.Any(d => d.UserId == userid))
             .Select(c => c)
             .ToListAsync();

            if (vfs != null && vfs.Any())
            {
                entities = vfs.Select(c => new VirtualFolderInfoModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    PhysicalFolderId = c.PhysicalFolderId,
                    IsShared = c.IsShared,
                    CreatedDate = c.CreatedDate,
                    IsStarred = c.VirtualFolderUsers.Any(c => c.IsStarred && c.UserId == userid),
                    Users = c.PhysicalFolder.PhysicalFolderUsers.Where(s => s != null).Select(cs => new UserModel
                    {
                        Email = cs.User?.Email ?? "",
                        RaleName = cs.User?.RaleName ?? "",
                        Id = cs.UserId,
                        IsOwner = cs.UserId == (c.PhysicalFolder?.CreatedBy ?? Guid.Empty),
                        ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath, cs.User?.ProfilePhoto ?? "")
                    }).ToList()

                }).ToList();
            }

            //追加更新目录
            if (entities != null && entities.Any())
                _folders.AddOrUpdate(entities);

            return entities;
        }
    }


    /// <summary>
    /// 获取指定用户父目录的虚拟目录
    /// </summary>
    /// <param name="folderIdsToReturn"></param>
    /// <returns></returns>
    public async Task<List<DocumentFolderModel>> GetDocumentVirtualFolders(Guid parentId, Guid userid)
    {
        var _pathHelper = new PathHelper();
        using (var context = _contextFactory.Create())
        {
            //转换此查询需要SQL APPLY操作，而SQLite不支持此操作。
            //Translating this query requires the SQL APPLY operation, which is not supported on SQLite.”

            var entities = new List<DocumentFolderModel>();

            var vfs = await context.VirtualFolders
             .Include(c => c.PhysicalFolder)
             .ThenInclude(c => c.PhysicalFolderUsers)
             .ThenInclude(c => c.User)
             .Where(c => c.ParentId == parentId && c.VirtualFolderUsers.Any(d => d.UserId == userid))
             .Select(c => c)
             .ToListAsync();

            if (vfs != null && vfs.Any())
            {
                entities = vfs.Select(c => new DocumentFolderModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    DocType = DocType.Folder,
                    VirtualFolderId = c.Id,
                    ParentId = c.ParentId,
                    PhysicalFolderId = c.PhysicalFolderId,
                    ThumbnailIcon = Symbol.FolderFilled,
                    IsShared = c.IsShared,
                    CreatedDate = c.CreatedDate,
                    IsView = false,
                    IsStarred = c.VirtualFolderUsers.Any(c => c.IsStarred && c.UserId == userid),
                    Users = c.PhysicalFolder.PhysicalFolderUsers.Where(s => s != null).Select(cs => new UserModel
                    {
                        Email = cs.User?.Email ?? "",
                        RaleName = cs.User?.RaleName ?? "",
                        Id = cs.UserId,
                        IsOwner = cs.UserId == (c.PhysicalFolder?.CreatedBy ?? Guid.Empty),
                        ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath, cs.User?.ProfilePhoto ?? "")
                    }).ToList()

                }).ToList();
            }

            return entities;
        }
    }


    public async Task<ServiceResult> DeleteVirtualFolder(Guid virtualFolderId, Guid userid)
    {
        using (var context = _contextFactory.Create())
        {
            try
            {
                var virtualFolderUser = await context.VirtualFolderUsers
                       .Where(c => (c.FolderId == virtualFolderId || c.VirtualFolder.PhysicalFolderId == virtualFolderId)
                           && c.UserId == userid)
                       .FirstOrDefaultAsync();
                if (virtualFolderUser == null)
                {
                    return ServiceResult.Fail("删除失败！");
                }

                var virtualFolderIdsToDelete = new List<Guid> { virtualFolderUser.FolderId };
                var virtualChildFolder = await GetChildsHierarchyById(virtualFolderUser.FolderId);
                virtualFolderIdsToDelete.AddRange(virtualChildFolder.Select(c => c.Id));

                var virtualFolderUsersToDelete = context.VirtualFolderUsers
                    .Where(c => c.UserId == userid && virtualFolderIdsToDelete.Contains(c.FolderId)).ToList();

                virtualFolderUsersToDelete.ForEach(user => user.IsDeleted = true);
                context.VirtualFolderUsers.UpdateRange(virtualFolderUsersToDelete);
                if (await context.SaveChangesAsync() <= 0)
                {
                    return ServiceResult.Fail("删除文档失败");
                }

                return ServiceResult.Ok("删除成功");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }
    }


    /// <summary>
    /// 删除回收站文件夹
    /// </summary>
    /// <param name="virtualFolderId"></param>
    /// <param name="userid"></param>
    /// <returns></returns>
    public async Task<ServiceResult> DeleteDeletedFolder(Guid virtualFolderId, Guid userid)
    {
        var pfus = Locator.Current.GetRequiredService<IPhysicalFolderUserService>();
        var pfs = Locator.Current.GetRequiredService<IPhysicalFolderService>();

        using (var context = _contextFactory.Create())
        {
            var virtualFolder = await context.VirtualFolders
                .IgnoreQueryFilters()
                .Include(cs => cs.PhysicalFolder)
                .Where(c => c.Id == virtualFolderId)
                .FirstOrDefaultAsync();

            if (virtualFolder == null)
            {
                return ServiceResult.Fail("删除失败！");
            }

            var isOwner = virtualFolder.CreatedBy == userid;
            var allPhysicalFolderIdsToDelete = new List<Guid>() { virtualFolder.PhysicalFolderId };
            var physicalChildFolders = await pfs.GetChildsHierarchyById(virtualFolder.PhysicalFolderId);

            allPhysicalFolderIdsToDelete.AddRange(physicalChildFolders.Select(c => c.Id).ToList());
            if (isOwner)
            {
                foreach (var physicalFolderId in allPhysicalFolderIdsToDelete)
                {
                    var vitrualFolders = context.VirtualFolders
                        .IgnoreQueryFilters()
                        .Where(c => c.PhysicalFolderId == physicalFolderId).ToList();

                    context.RemoveRange(vitrualFolders);

                    var physicalFolders = await context.PhysicalFolders
                        .IgnoreQueryFilters()
                        .Where(c => c.Id == physicalFolderId)
                        .ToListAsync();

                    context.RemoveRange(physicalFolders);

                }
            }
            else
            {
                foreach (var physicalFolderId in allPhysicalFolderIdsToDelete)
                {
                    var virtualFolderUser = await context.VirtualFolderUsers
                          .IgnoreQueryFilters()
                          .Where(c => c.VirtualFolder.PhysicalFolderId == physicalFolderId
                            && c.UserId == userid)
                          .ToListAsync();
                    context.RemoveRange(virtualFolderUser);


                    var physicalFolderUsers = context.PhysicalFolderUsers
                        .IgnoreQueryFilters()
                        .Where(c => c.UserId == userid && c.FolderId == physicalFolderId)
                        .ToList();
                    context.RemoveRange(physicalFolderUsers);


                }
            }
            var physicalFolderPath = await pfs.GetParentFolderPath(virtualFolder.PhysicalFolder.Id);
            if (await context.SaveChangesAsync() <= 0)
            {
                return ServiceResult.Fail("删除文档失败");
            }

            // Delete Folder from Disk
            if (isOwner)
            {
                var _pathHelper = new PathHelper();
                var fullcontainerDocumentPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, physicalFolderPath);
                if (Directory.Exists(fullcontainerDocumentPath))
                {
                    try
                    {
                        Directory.Delete(fullcontainerDocumentPath, true);
                    }
                    catch (Exception e)
                    {
                        return ServiceResult.Fail(e.Message);
                    }
                }
            }

            return ServiceResult.Ok("删除成功");
        }

    }


    /// <summary>
    /// 获取虚拟文件夹明细
    /// </summary>
    public async Task<VirtualFolderInfoModel> GetVirtualFolderDetailById(Guid virtualFolderId, Guid userId)
    {
        var _pathHelper = new PathHelper();
        using (var context = _contextFactory.Create())
        {
            var c = await context.VirtualFolders
                  .Include(c => c.PhysicalFolder)
                  .ThenInclude(c => c.PhysicalFolderUsers)
                  .Where(c => c.Id == virtualFolderId && c.PhysicalFolder.PhysicalFolderUsers.Any(phu => phu.UserId == userId))
                  .Select(c => c)
                  .FirstOrDefaultAsync();

            if (c != null)
            {
                return new VirtualFolderInfoModel
                {
                    Id = c?.Id ?? Guid.Empty,
                    Name = c?.Name ?? "",
                    ParentId = c?.ParentId ?? Guid.Empty,
                    PhysicalFolderId = c?.PhysicalFolderId ?? Guid.Empty,
                    IsShared = c?.IsShared ?? false,
                    CreatedDate = c.CreatedDate,
                    Users = c.PhysicalFolder.PhysicalFolderUsers.Where(s => s != null).Select(cs => new UserModel
                    {
                        Email = cs.User?.Email ?? "",
                        RaleName = cs.User?.RaleName ?? "",
                        Id = cs.UserId,
                        IsOwner = cs.UserId == (c.PhysicalFolder?.CreatedBy ?? Guid.Empty),
                        ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath, cs.User?.ProfilePhoto ?? "")
                    }).ToList()
                };
            }
            else
            {
                return null;
            }
        }
    }
}

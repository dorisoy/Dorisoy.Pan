using Microsoft.EntityFrameworkCore;
using Dorisoy.Pan.Data.Contexts;
using Path = System.IO.Path;

namespace Dorisoy.Pan.Services;

public class DocumentService : IDocumentService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;

    private readonly IUsersService _usersService;
    private readonly IVirtualFolderService _virtualFolderService;

    private readonly SourceCache<DocumentFolderModel, Guid> _items;
    public IObservable<IChangeSet<DocumentFolderModel, Guid>> Connect() => _items.Connect();


    private readonly IObservable<bool> _isLoading;
    public IObservable<bool> IsLoading => _isLoading;


    public DocumentService(IDbContextFactory<CaptureManagerContext> contextFactory, IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;
        _usersService = Locator.Current.GetService<IUsersService>();
        _virtualFolderService = Locator.Current.GetService<IVirtualFolderService>();

        _items = new SourceCache<DocumentFolderModel, Guid>(e => e.Id);

    }

    /// <summary>
    /// 获取文档
    /// </summary>
    /// <returns></returns>
    public async Task<List<DocumentModel>> GetDocuments()
    {
        return await Task.Run(() =>
        {
            var documents = new List<DocumentModel>();
            using (var context = _contextFactory.Create())
            {
                var result = _mapper.ProjectTo<DocumentModel>(context.Documents);
                documents = result.ToList();
            }
            return documents;
        });
    }



    /// <summary>
    /// 获取项目文档总计
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetDocumentsCount(Guid userId, string ext = "")
    {
        using (var context = _contextFactory.Create())
        {
            try
            {
                var query = context.Documents.Where(s => s.CreatedBy == userId);

                if (!string.IsNullOrEmpty(ext))
                    query = query.Where(s => s.Extension.Contains(ext));

                return await query.CountAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }


    /// <summary>
    /// 获取项目文档
    /// </summary>
    /// <returns></returns>
    public async Task<List<DocumentModel>> GetPatienterDocuments(Guid userId, Guid patienterId, string ext = "")
    {
        return await Task.Run(async () =>
        {
            var documents = new List<DocumentModel>();
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var query = context.Documents.Where(s => s.CreatedBy == userId && s.PatienterId == patienterId);

                    if (!string.IsNullOrEmpty(ext))
                        query = query.Where(s => s.Extension.Contains(ext));

                    var result = _mapper.ProjectTo<DocumentModel>(query);

                    var pds = await result.OrderByDescending(s => s.CreatedDate).ToListAsync();

                    var dcs = await context.DocumentComments.Where(c => c.CreatedBy == userId).ToListAsync();

                    pds.ForEach(s =>
                    {
                        s.DocComments = dcs.Where(c => c.DocumentId == s.Id).ToList();
                        s.Comments = s.DocComments.Count();
                    });

                    documents = pds;
                }
                catch (Exception ex)
                {

                }
            }
            return documents;
        });
    }



    /// <summary>
    /// 根据目录获取全部文档
    /// </summary>
    /// <param name="folderId"></param>
    /// <returns></returns>
    public async Task<List<DocumentFolderModel>> GetAllDocuments(Guid folderId)
    {
        return await Task.Run(async () =>
        {
            _items.Clear();

            var _userInfoToken = Globals.CurrentUser;
            var vmodels = new List<DocumentFolderModel>();
            using (var context = _contextFactory.Create())
            {
                #region 目录

                var vfms = await _virtualFolderService.GetDocumentVirtualFolders(folderId, _userInfoToken.Id);
                //追加目录
                if (vfms != null && vfms.Any())
                    vmodels.AddRange(vfms);

                #endregion

                #region  文档

                var _pathHelper = new PathHelper();
                var user = await _usersService.FindByIdAsync(_userInfoToken.Id);
                //当前虚拟目录
                var folder = await _virtualFolderService.FindAsync(folderId);
                if (folder == null || folder.ParentId == null)
                {
                    var models = await context.Documents
                    .IgnoreQueryFilters()
                    .Where(c => c.PhysicalFolderId == folder.PhysicalFolderId
                       && c.CreatedBy == _userInfoToken.Id
                       && !c.DocumentDeleteds.Any(cs => cs.UserId == _userInfoToken.Id))
                    .Select(c => new DocumentFolderModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        DocType = DocType.File,
                        Extension = c.Extension,
                        VirtualFolderId = Guid.Empty,
                        PhysicalFolderId = c.PhysicalFolderId,
                        ThumbnailPath = Path.Combine(_pathHelper.DocumentPath, c.ThumbnailPath),
                        ThumbnailIcon = Symbol.OpenFile,
                        ModifiedDate = c.ModifiedDate.Value,
                        CreatedDate = c.CreatedDate,
                        Size = c.Size,
                        IsView = true,
                        DeletedUserIds = c.DocumentDeleteds.Where(cs => cs.IsDeleted).Select(c => c.UserId).ToList()
                    })
                  .OrderByDescending(c => c.ModifiedDate)
                  .ToListAsync();

                    if (models != null && models.Any())
                        vmodels.AddRange(models);
                }
                else
                {
                    var query = await context.Documents
                    .IgnoreQueryFilters()
                    .Where(c => c.PhysicalFolderId == folder.PhysicalFolderId
                      && !c.DocumentDeleteds.Any(cs => cs.UserId == _userInfoToken.Id))
                    .Where(s => s != null).ToListAsync();

                    if (query != null && query.Any())
                    {
                        var models = query.Select(c => new DocumentFolderModel
                        {
                            Id = c?.Id ?? Guid.Empty,
                            Name = c?.Name ?? "",
                            DocType = DocType.File,
                            Extension = c?.Extension ?? "",
                            VirtualFolderId = Guid.Empty,
                            PhysicalFolderId = c?.PhysicalFolderId ?? Guid.Empty,
                            ThumbnailPath = Path.Combine(_pathHelper.DocumentPath, c.ThumbnailPath),
                            ThumbnailIcon = Symbol.OpenFile,
                            ModifiedDate = c.ModifiedDate.Value,
                            CreatedDate = c.CreatedDate,
                            Size = c.Size,
                            IsView = true,
                            DeletedUserIds = c?.DocumentDeleteds?.Where(cs => cs.IsDeleted).Select(c => c.UserId).ToList(),
                            PhysicalUsers = c?.Folder?.PhysicalFolderUsers?.Select(cs => new UserModel
                            {
                                Id = cs?.UserId ?? Guid.Empty,
                                Email = cs?.User?.Email ?? "",
                                RaleName = cs?.User?.RaleName ?? "",
                                IsOwner = c?.CreatedBy == cs?.UserId,
                                ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath, cs?.User?.ProfilePhoto ?? "")
                            }).ToList()
                        })
                        .OrderByDescending(c => c.ModifiedDate)
                        .ToList();

                        if (models != null && models.Any())
                            vmodels.AddRange(models);
                    }
                }

                //vmodels.ForEach(entity =>
                //{
                //    var userIds = entity.Users.Select(c => c.Id);
                //    var usersToAdd = entity.PhysicalUsers.Where(c => !userIds.Contains(c.Id)).ToList();
                //    entity.Users.AddRange(usersToAdd);
                //    entity.PhysicalUsers = null;
                //    entity.Users = entity.Users.Where(c => !entity.DeletedUserIds.Contains(c.Id)).ToList();
                //});

                #endregion
            }

            //按文件类型和创建时间倒序排序
            //vmodels = vmodels
            //.OrderByDescending(s => s.DocType)
            //.ThenByDescending(s => s.CreatedDate)
            //.ToList();

            if (vmodels.Any())
                _items.AddOrUpdate(vmodels);



            return vmodels;
        });
    }

    /// <summary>
    /// 获取文档
    /// </summary>
    /// <param name="parentId"></param>
    /// <param name="fileName"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<DocumentModel> GetDocument(Guid parentId, string fileName, Guid userId)
    {
        return await Task.Run(async () =>
        {
            var document = new DocumentModel();
            using (var context = _contextFactory.Create())
            {

                var result = await context.Documents.Where(c => c.PhysicalFolderId == parentId
                && c.Name == fileName
                && (c.CreatedBy == userId || c.Folder.PhysicalFolderUsers.Any(c => c.UserId == userId)))
                .FirstOrDefaultAsync();
                document = _mapper.Map<DocumentModel>(result);
            }
            return document;
        });
    }


    /// <summary>
    /// 添加文档
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ServiceResult<DocumentModel>> AddAsync(DocumentModel model, bool update = true)
    {
        return await Task.Run(() =>
        {
            var user = Globals.CurrentUser;
            using (var context = _contextFactory.Create())
            {
                var entity = new Document();
                entity.CreatedBy = user.Id;
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedBy = user.Id;
                entity.ModifiedDate = DateTime.Now;

                _mapper.Map(model, entity);

                context.Documents.Add(entity);
                context.SaveChanges();

                _mapper.Map(entity, model);

                if (update)
                {
                    var dfm = _mapper.Map<DocumentFolderModel>(model);
                    _items.AddOrUpdate(dfm);
                }

                //_items.Refresh();

                return ServiceResult.Ok(model);
            }
        });
    }
    public async Task<ServiceResult<DocumentModel>> UpdateAsync(DocumentModel model)
    {
        return await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Documents.First(e => e.Id == model.Id);
                _mapper.Map(model, entity);

                context.SaveChanges();

                _mapper.Map(entity, model);

                var dfm = _mapper.Map<DocumentFolderModel>(model);
                _items.AddOrUpdate(dfm);

                return ServiceResult.Ok(model);
            }
        });
    }
    public async Task DeleteAsync(DocumentModel model)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Set<Document>().Find(model.Id);
                if (entity != null)
                {
                    context.Entry(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }
    public bool DocumentnameIsFree(Guid id, string documentname)
    {
        using (var context = _contextFactory.Create())
        {
            var count = context.Documents.Count(e => e.Id == id && e.Name == documentname) > 1;
            return !count;
        }
    }

    /// <summary>
    /// 删除文档
    /// </summary>
    /// <param name="documentId"></param>
    /// <returns></returns>
    public async Task<ServiceResult> DeleteDocument(Guid documentId)
    {
        using (var context = _contextFactory.Create())
        {
            var user = Globals.CurrentUser;
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
            {
                return ServiceResult.Fail("没找到文档");
            }

            var deletedDocument = context.DocumentDeleteds
                .FirstOrDefault(c => c.UserId == user.Id && c.DocumentId == documentId);

            if (deletedDocument == null)
            {
                context.DocumentDeleteds.Add(new Data.DocumentDeleted
                {
                    DocumentId = documentId,
                    ModifiedBy = user.Id,
                    ModifiedDate = DateTime.Now,
                    UserId = user.Id
                });

                if (await context.SaveChangesAsync() <= 0)
                {
                    return ServiceResult.Fail("删除文档失败");
                }
            }

            return ServiceResult.Ok("删除文件到回收站");
        }
    }

    /// <summary>
    /// 删除回收站文档
    /// </summary>
    /// <param name="documentId"></param>
    /// <param name="userid"></param>
    /// <returns></returns>
    public async Task<ServiceResult> DeleteDeletedDocument(Guid documentId, Guid userid)
    {
        var pfs = Locator.Current.GetRequiredService<IPhysicalFolderService>();

        using (var context = _contextFactory.Create())
        {
            var user = Globals.CurrentUser;
            var document = await context.Documents
               .IgnoreQueryFilters()
               .FirstOrDefaultAsync(c => c.Id == documentId);

            if (document == null)
            {
                return ServiceResult.Fail("没找到文档");
            }

            var isOwner = document.CreatedBy == user.Id;
            if (isOwner)
            {

                var deletedUsers = await context.DocumentDeleteds
                    .IgnoreQueryFilters()
                     .Where(c => c.DocumentId == documentId)
                     .ToArrayAsync();

                context.DocumentDeleteds.RemoveRange(deletedUsers);
                context.Documents.Remove(document);
            }
            else
            {
                var deletedUsers = await context.DocumentDeleteds
                    .IgnoreQueryFilters()
                    .Where(c => c.DocumentId == documentId && c.UserId == user.Id)
                    .ToArrayAsync();

                context.DocumentDeleteds.RemoveRange(deletedUsers);
            }

            if (await context.SaveChangesAsync() <= 0)
            {
                return ServiceResult.Fail("Error while deleting the document");
            }

            if (isOwner)
            {
                var _pathHelper = new PathHelper();
                // delete document
                var fullDocumentpath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, document.Path);
                if (File.Exists(fullDocumentpath))
                {
                    try
                    {
                        File.Delete(fullDocumentpath);
                    }
                    catch
                    {
                        return ServiceResult.Fail("Error while deleting the document from dis");
                    }
                }

                // delete document version
                var rootPath = await pfs.GetParentFolderPath(document.PhysicalFolderId);
                var versionFolder = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, user.Id.ToString(), document.Id.ToString());
                if (Directory.Exists(versionFolder))
                {
                    try
                    {
                        Directory.Delete(versionFolder, true);
                    }
                    catch
                    {
                        return ServiceResult.Fail("Error while deleting the document");

                    }
                }
            }

            return ServiceResult.Ok("删除回收站文档成功！");
        }
    }
}

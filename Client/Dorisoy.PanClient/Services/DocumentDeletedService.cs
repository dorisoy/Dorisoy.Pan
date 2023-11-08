using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Dorisoy.PanClient.Common;
using Dorisoy.PanClient.Data.Contexts;
using Dorisoy.PanClient.Utils;


namespace Dorisoy.PanClient.Services;

public class DocumentDeletedService : IDocumentDeletedService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;

    private readonly IUsersService _usersService;
    private readonly IVirtualFolderService _virtualFolderService;


    private readonly SourceCache<DocumentFolderModel, Guid> _items;
    public IObservable<IChangeSet<DocumentFolderModel, Guid>> Connect() => _items.Connect();


    public DocumentDeletedService(IDbContextFactory<CaptureManagerContext> contextFactory, IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;
        _usersService = Locator.Current.GetService<IUsersService>();
        _virtualFolderService = Locator.Current.GetService<IVirtualFolderService>();

        _items = new SourceCache<DocumentFolderModel, Guid>(e => e.Id);
    }


    public async Task<List<DeletedDocumentInfoModel>> GetAllDeletedDocuments()
    {
        return await Task.Run(async () =>
        {
            var user = Globals.CurrentUser;
            using (var context = _contextFactory.Create())
            {
                var _pathHelper = new PathHelper();
                var deletedDocuments = await context.DocumentDeleteds
                .Where(c => c.UserId == user.Id)
                  .Select(cs => new DeletedDocumentInfoModel
                  {
                      Id = cs.Document.Id,
                      DeletedDate = cs.CreatedDate,
                      Name = cs.Document.Name,
                      ThumbnailPath = Path.Combine(_pathHelper.DocumentPath, cs.Document.ThumbnailPath),
                  }).ToListAsync();

                return deletedDocuments;
            }
        });
    }

    /// <summary>
    /// 删除文档
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task DeleteAsync(DeletedDocumentInfoModel model)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                var entity = context.Set<DocumentDeleted>().Find(model.Id);
                if (entity != null)
                {
                    context.Entry(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }


    /// <summary>
    /// 删除文件到回收站
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
    /// 还原回收站
    /// </summary>
    /// <param name="documentId"></param>
    /// <returns></returns>
    public async Task<ServiceResult> RestoreDeletedDocument(Guid documentId)
    {
        using (var context = _contextFactory.Create())
        {
            var user = Globals.CurrentUser;
            var documentPermission = await context.DocumentDeleteds
                .Where(c => c.UserId == user.Id && c.DocumentId == documentId)
               .FirstOrDefaultAsync();

            if (documentPermission != null)
            {
                context.DocumentDeleteds.Remove(documentPermission);
                if (await context.SaveChangesAsync() <= 0)
                {
                    return ServiceResult.Fail("还原回收站");
                }
            }

            return ServiceResult.Ok("还原回收站成功");
        }
    }


}

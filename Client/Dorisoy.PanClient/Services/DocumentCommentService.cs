using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Dorisoy.PanClient.Common;
using Dorisoy.PanClient.Data.Contexts;


namespace Dorisoy.PanClient.Services;

public class DocumentCommentService : IDocumentCommentService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;

    private readonly SourceCache<DocumentCommentModel, Guid> _dcs;
    public IObservable<IChangeSet<DocumentCommentModel, Guid>> Connect() => _dcs.Connect();


    public DocumentCommentService(IDbContextFactory<CaptureManagerContext> contextFactory,
        IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;

        _dcs = new SourceCache<DocumentCommentModel, Guid>(e => e.Id);

        //using (var context = _contextFactory.Create())
        //{
        //    var users = _mapper.ProjectTo<DocumentCommentModel>(context.DocumentComments);
        //    _dcs.AddOrUpdate(users);
        //}
    }


    public async Task<Result<List<DocumentCommentModel>>> GetAllAsync()
    {
        var dcs = new List<DocumentCommentModel>();
        using (var context = _contextFactory.Create())
        {
            var result = _mapper.ProjectTo<DocumentCommentModel>(context.DocumentComments);
            dcs = result.ToList();
        }
        return await Result<List<DocumentCommentModel>>.SuccessAsync(dcs);
    }


    public async Task<Result<List<DocumentCommentModel>>> GetAllDocumentComment(Guid documentId)
    {
        _dcs.Clear();
        var dcs = new List<DocumentCommentModel>();
        using (var context = _contextFactory.Create())
        {
            var docs = await context.DocumentComments.Where(s => s.DocumentId == documentId)
                .OrderBy(c => c.CreatedDate)
                .Select(c => new DocumentCommentModel
                {
                    Id = c.Id,
                    DocumentId = c.DocumentId,
                    Comment = c.Comment,
                    CreatedDate = c.CreatedDate,
                    UserName = $"{c.CreatedByUser.RaleName}"
                }).ToListAsync();

            dcs = docs;
            _dcs.AddOrUpdate(docs);
        }
        return await Result<List<DocumentCommentModel>>.SuccessAsync(dcs);
    }



    public async Task<Result<DocumentCommentModel>> GetByIdAsync(Guid id)
    {
        using (var context = _contextFactory.Create())
        {
            var dcs = await context.DocumentComments.SingleOrDefaultAsync(x => x.Id == id);
            var dcsResponse = _mapper.Map<DocumentCommentModel>(dcs);
            return await Result<DocumentCommentModel>.SuccessAsync(dcsResponse);
        }
    }


    public async Task<DocumentComment> FindByIdAsync(Guid id)
    {
        using (var context = _contextFactory.Create())
        {
            var dcs = await context.DocumentComments.SingleOrDefaultAsync(x => x.Id == id);
            return dcs;
        }
    }


    public async Task<Result<int>> DeleteAsync(Guid id)
    {
        using (var context = _contextFactory.Create())
        {
            var dc = await context.DocumentComments.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (dc != null)
            {
                context.Entry(dc).State = EntityState.Deleted;
                var ret = await context.SaveChangesAsync();
                return await Result<int>.SuccessAsync(ret);
            }
            else
                return await Result<int>.SuccessAsync(0);
        }
    }


    public async Task<ServiceResult<DocumentCommentModel>> AddAsync(DocumentCommentModel model)
    {
        return await Task.Run(() =>
        {
            var user = Globals.CurrentUser;
            using (var context = _contextFactory.Create())
            {
                var entity = new DocumentComment
                {
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.Id,
                    ModifiedBy = user.Id,
                    ModifiedDate = DateTime.Now
                };

                _mapper.Map(model, entity);

                context.DocumentComments.Add(entity);
                context.SaveChanges();

                _mapper.Map(entity, model);

                _dcs.AddOrUpdate(model);

                return ServiceResult.Ok(model);
            }
        });
    }

    public async Task DeleteAsync(DocumentComment dc)
    {
        await Task.Run(() =>
        {
            using (var context = _contextFactory.Create())
            {
                if (dc != null)
                {
                    context.Entry(dc).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        });
    }



}

using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class AddDocumentCommentCommandHandler : IRequestHandler<AddDocumentCommentCommand, ServiceResponse<DocumentCommentDto>>
    {
        private readonly IDocumentCommentRepository _documentCommentRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;

        public AddDocumentCommentCommandHandler(IDocumentCommentRepository documentCommentRepository,
            IUnitOfWork<DocumentContext> uow)
        {
            _documentCommentRepository = documentCommentRepository;
            _uow = uow;
        }

        public async Task<ServiceResponse<DocumentCommentDto>> Handle(AddDocumentCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = new Data.DocumentComment
            {
                Comment = request.Comment,
                DocumentId = request.DocumentId
            };
            _documentCommentRepository.Add(comment);

            if (await _uow.SaveAsync() <= 0)
            {
                return ServiceResponse<DocumentCommentDto>.Return500();
            }

            var commentInfo = await _documentCommentRepository.All
                .Where(c => c.Id == comment.Id)
                .Select(c => new DocumentCommentDto
                {
                    Comment = c.Comment,
                    CreatedDate = c.CreatedDate,
                    UserName = $"{c.CreatedByUser.FirstName} {c.CreatedByUser.LastName}"
                }).FirstOrDefaultAsync();

            return ServiceResponse<DocumentCommentDto>.ReturnResultWith201(commentInfo);
        }
    }
}

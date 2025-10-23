using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class AddLogCommandHandler : IRequestHandler<AddLogCommand, ServiceResponse<NLogDto>>
    {
        private readonly INLogRespository _nLogRespository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        public AddLogCommandHandler(
           INLogRespository nLogRespository,
            IUnitOfWork<DocumentContext> uow
            )
        {
            _nLogRespository = nLogRespository;
            _uow = uow;
        }
        public async Task<ServiceResponse<NLogDto>> Handle(AddLogCommand request, CancellationToken cancellationToken)
        {
            _nLogRespository.Add(new NLog
            {
                Id = Guid.NewGuid(),
                Logged = DateTime.UtcNow,
                Level = "Error",
                Message = request.ErrorMessage,
                Source = "Angular",
                Exception = request.Stack
            });
            await _uow.SaveAsync();
            return ServiceResponse<NLogDto>.ReturnSuccess();
        }
    }
}

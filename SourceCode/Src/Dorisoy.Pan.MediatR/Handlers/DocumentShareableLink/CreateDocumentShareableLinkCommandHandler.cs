using AutoMapper;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class CreateDocumentShareableLinkCommandHandler
        : IRequestHandler<CreateDocumentShareableLinkCommand, ServiceResponse<DocumentShareableLinkDto>>
    {
        private readonly IMapper _mapper;
        private readonly IDocumentShareableLinkRepository _documentShareableLinkRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;

        public CreateDocumentShareableLinkCommandHandler(IMapper mapper,
            IDocumentShareableLinkRepository documentShareableLinkRepository,
            IUnitOfWork<DocumentContext> uow)
        {
            _mapper = mapper;
            _documentShareableLinkRepository = documentShareableLinkRepository;
            _uow = uow;
        }
        public async Task<ServiceResponse<DocumentShareableLinkDto>> Handle(CreateDocumentShareableLinkCommand request, CancellationToken cancellationToken)
        {
            DocumentShareableLink sharableLInk;
            if (request.Id.HasValue)
            {
                sharableLInk = _mapper.Map<DocumentShareableLink>(request);
                if (!string.IsNullOrWhiteSpace(sharableLInk.Password))
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(sharableLInk.Password);
                    sharableLInk.Password = Convert.ToBase64String(plainTextBytes);
                }
                if (sharableLInk.LinkExpiryTime.HasValue)
                {
                    var date = sharableLInk.LinkExpiryTime.Value;
                    sharableLInk.LinkExpiryTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
                }
                _documentShareableLinkRepository.Update(sharableLInk);
            }
            else
            {
                sharableLInk = _mapper.Map<DocumentShareableLink>(request);
                sharableLInk.LinkCode = Guid.NewGuid().ToString();
                if (!string.IsNullOrWhiteSpace(sharableLInk.Password))
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(sharableLInk.Password);
                    sharableLInk.Password = Convert.ToBase64String(plainTextBytes);
                }
                if (sharableLInk.LinkExpiryTime.HasValue)
                {
                    var date = sharableLInk.LinkExpiryTime.Value;
                    sharableLInk.LinkExpiryTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
                }
                _documentShareableLinkRepository.Add(sharableLInk);
            }

            if (await _uow.SaveAsync() <= 0)
            {
                return ServiceResponse<DocumentShareableLinkDto>.Return500();
            }
            var result = _mapper.Map<DocumentShareableLinkDto>(sharableLInk);
            if (!string.IsNullOrWhiteSpace(result.Password))
            {
                var base64EncodedBytes = Convert.FromBase64String(result.Password);
                result.Password = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            return ServiceResponse<DocumentShareableLinkDto>.ReturnResultWith200(result);
        }
    }
}

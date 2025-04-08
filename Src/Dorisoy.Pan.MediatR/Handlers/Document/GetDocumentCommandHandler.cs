using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dorisoy.Pan.MediatR.Handlers;

public class GetDocumentCommandHandler: IRequestHandler<GetDocumentCommand, DocumentDto>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly UserInfoToken _userInfoToken;
    private readonly IMapper _mapper;
    public GetDocumentCommandHandler(IDocumentRepository documentRepository,
        IMapper mapper,
        UserInfoToken userInfoToken)
    {
        _documentRepository = documentRepository;
        _userInfoToken = userInfoToken;
        _mapper = mapper;
    }

    public async Task<DocumentDto> Handle(GetDocumentCommand request, CancellationToken cancellationToken)
    {
        var doc = await _documentRepository.All.Where(p => p.Id == request.DocumentId)
            .FirstOrDefaultAsync(cancellationToken);
        return _mapper.Map<DocumentDto>(doc);
    }
}
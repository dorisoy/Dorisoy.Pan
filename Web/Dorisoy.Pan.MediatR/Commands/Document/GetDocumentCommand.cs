using System;
using Dorisoy.Pan.Data.Dto;
using MediatR;

namespace Dorisoy.Pan.MediatR.Commands;

public class GetDocumentCommand: IRequest<DocumentDto>
{
    public Guid DocumentId { get; set; }
}
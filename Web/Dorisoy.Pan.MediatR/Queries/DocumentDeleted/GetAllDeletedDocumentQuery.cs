using Dorisoy.Pan.Data.Dto;
using MediatR;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetAllDeletedDocumentQuery : IRequest<List<DeletedDocumentInfoDto>>
    {
    }
}

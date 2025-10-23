using Dorisoy.Pan.Data.Dto;
using MediatR;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetLinkInfoByCodeQuery : IRequest<DocumentShareableLinkDto>
    {
        public string Code { get; set; }
    }
}

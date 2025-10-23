using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetDocumentTokenQuery : IRequest<string>
    {
        public Guid Id { get; set; }
        public bool IsVersion { get; set; }
    }
}

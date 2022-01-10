using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetParentChildSharedQuery : IRequest<HierarchySharedDto>
    {
        public Guid Id { get; set; }
    }
}

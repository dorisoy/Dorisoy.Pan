using Dorisoy.Pan.Data;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetFolderParentsByIdQuery: IRequest<IEnumerable<HierarchyFolder>>
    {
        public Guid Id { get; set; }
    }
}

using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetAllDocumentsQuery : IRequest<List<DocumentDto>>
    {
        public Guid FolderId { get; set; }
    }
}

using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetAllDocumentCommentQuery : IRequest<List<DocumentCommentDto>>
    {
        public Guid Id { get; set; }
    }
}

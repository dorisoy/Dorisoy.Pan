using MediatR;
using System;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetLogQuery : IRequest<ServiceResponse<NLogDto>>
    {
        public Guid Id { get; set; }
    }
}

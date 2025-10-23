using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class DownloadFolderCommand : IRequest<List<DownloadDocumentDto>>
    {
        public Guid Id { get; set; }
    }
}

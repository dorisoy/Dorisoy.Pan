using Dorisoy.Pan.Data.Dto;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class DownloadDocumentAndFolderCommand : IRequest<List<DownloadDocumentDto>>
    {
        public List<Guid> FolderIds { get; set; } = new List<Guid>();
        public List<Guid> DocumentIds { get; set; } = new List<Guid>();
    }
}

using MediatR;
using System;

namespace Dorisoy.Pan.MediatR.Commands
{
    public class DownloadDocumentCommand : IRequest<string>
    {
        public Guid Id { get; set; }
        public bool IsVersion { get; set; }
        public bool IsFromPreview { get; set; } = false;
        public string Token { get; set; }
    }
}

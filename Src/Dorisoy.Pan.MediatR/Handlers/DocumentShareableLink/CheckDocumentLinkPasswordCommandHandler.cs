using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class CheckDocumentLinkPasswordCommandHandler
        : IRequestHandler<CheckDocumentLinkPasswordCommand, bool>
    {
        private readonly IDocumentShareableLinkRepository _documentShareableLinkRepository;

        public CheckDocumentLinkPasswordCommandHandler(IDocumentShareableLinkRepository documentShareableLinkRepository)
        {
            _documentShareableLinkRepository = documentShareableLinkRepository;
        }
        public async Task<bool> Handle(CheckDocumentLinkPasswordCommand request, CancellationToken cancellationToken)
        {
            var link = await _documentShareableLinkRepository.All.FirstOrDefaultAsync(c => c.Id == request.Id);
            if (link == null)
            {
                return false;
            }

            var base64EncodedBytes = Convert.FromBase64String(link.Password);
            var password = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            return password == request.Password;
        }
    }
}

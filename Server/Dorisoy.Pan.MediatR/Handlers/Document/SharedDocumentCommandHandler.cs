using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class SharedDocumentCommandHandler : IRequestHandler<SharedDocumentCommand, bool>
    {
        private readonly IDocumentSharedUserRepository _documentSharedUserRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IUserNotificationRepository _userNotificationRepository;
        public SharedDocumentCommandHandler(
            IDocumentSharedUserRepository documentSharedUserRepository,
            IUnitOfWork<DocumentContext> uow,
            IUserNotificationRepository userNotificationRepository
            )
        {
            _documentSharedUserRepository = documentSharedUserRepository;
            _uow = uow;
            _userNotificationRepository = userNotificationRepository;
        }

        public async Task<bool> Handle(SharedDocumentCommand request, CancellationToken cancellationToken)
        {
            List<SharedDocumentUser> lstSharedDocumentUser = new List<SharedDocumentUser>();
            foreach (var user in request.Users)
            {
                if (! await _documentSharedUserRepository.All.AnyAsync(c => c.UserId == user && c.DocumentId == request.Id))
                {
                    lstSharedDocumentUser.Add(new SharedDocumentUser
                    {
                        DocumentId = request.Id,
                        UserId = user
                    });
                }
            }
            if (lstSharedDocumentUser.Count() > 0)
            {
                _documentSharedUserRepository.AddRange(lstSharedDocumentUser);
                 _userNotificationRepository.SaveUserNotification(null, request.Id, request.Users, ActionEnum.Shared);
                if (await _uow.SaveAsync() <= 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

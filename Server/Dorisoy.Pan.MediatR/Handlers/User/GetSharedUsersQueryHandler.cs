using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetSharedUsersQueryHandler : IRequestHandler<GetSharedUsersQuery, UserList>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IDocumentSharedUserRepository _documentSharedUserRepository;
        private readonly IDocumentRepository _documentRepository;
        public GetSharedUsersQueryHandler(IUserRepository userRepository,
            IVirtualFolderRepository virtualFolderRepository,
            IDocumentSharedUserRepository documentSharedUserRepository,
            IDocumentRepository documentRepository)
        {
            _userRepository = userRepository;
            _virtualFolderRepository = virtualFolderRepository;
            _documentSharedUserRepository = documentSharedUserRepository;
            _documentRepository = documentRepository;
        }

        public async Task<UserList> Handle(GetSharedUsersQuery request, CancellationToken cancellationToken)
        {
            var folderUsers = new List<Guid>();
            var documentUsers = new List<Guid>();
            if (request.UserResource.Type.ToLower() == "folder")
            {
                folderUsers = await _virtualFolderRepository.GetFolderUserByPhysicalFolderId(Guid.Parse(request.UserResource.FolderId));
            }
            else
            {
                var documentId = Guid.Parse(request.UserResource.DocumentId);
                documentUsers = await _documentSharedUserRepository.All
                    .Where(c => c.DocumentId == documentId)
                    .Select(c => c.UserId)
                    .ToListAsync();
                var owner = await _documentRepository.All
                    .Where(c => c.Id == documentId)
                    .Select(c => c.CreatedBy.Value).ToListAsync();
                documentUsers.AddRange(owner);
            }
            return await _userRepository.GetSharedUsers(request.UserResource, folderUsers, documentUsers);
        }
    }
}

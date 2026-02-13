using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository
{
    public class VirtualFolderUserRepository : GenericRepository<VirtualFolderUser, DocumentContext>,
           IVirtualFolderUserRepository
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        public VirtualFolderUserRepository(
            IUnitOfWork<DocumentContext> uow,
            UserInfoToken userInfoToken,
            IVirtualFolderRepository virtualFolderRepository
            ) : base(uow)
        {
            _userInfoToken = userInfoToken;
            _virtualFolderRepository = virtualFolderRepository;
        }
        public void AssignPermission(Guid id, Guid folderId)
        {

            var data = All.Where(c => c.FolderId == id).Select(c => c.UserId).ToList();
            if (data.Any())
            {
                var virtualFolderUsers = data.Select(c => new VirtualFolderUser
                {
                    FolderId = folderId,
                    UserId = c
                }).ToList();

                AddRange(virtualFolderUsers);
            }
            else
            {
                Add(new VirtualFolderUser
                {
                    UserId = _userInfoToken.Id,
                    FolderId = folderId
                });
            }

        }
        public void AddFolderUsers(Guid folderId, List<Guid> users)
        {
            var lstFolderUsers = new List<VirtualFolderUser>();
            foreach (var userId in users)
            {
                if (!All.Any(c => c.FolderId == folderId && c.UserId == userId))
                {
                    lstFolderUsers.Add(new VirtualFolderUser
                    {
                        UserId = userId,
                        FolderId = folderId
                    });
                }
            }
            if (lstFolderUsers.Count() > 0)
            {
                AddRange(lstFolderUsers);
            }
        }
        public async Task AddVirtualFolderUsersChildsById(Guid id, List<Guid> users)
        {
            this.AddFolderUsers(id, users);
            var virtualChildFolders = await _virtualFolderRepository.GetChildsHierarchyById(id);
            if (virtualChildFolders.Count() > 0)
            {
                foreach (var virutalChildFolder in virtualChildFolders)
                {
                    this.AddFolderUsers(virutalChildFolder.Id, users);
                }
            }
        }
        public async Task RemovedFolderUsers(List<HierarchyFolder> lstFolders, Guid userId)
        {
            var lstFolderUsers = new List<VirtualFolderUser>();
            foreach (var virtualFolder in lstFolders)
            {
                var virtualFolderUser = await All.Where(c => c.FolderId == virtualFolder.Id && c.UserId == userId).FirstOrDefaultAsync();
                if (virtualFolderUser != null)
                {
                    lstFolderUsers.Add(virtualFolderUser);
                }
            }
            if (lstFolderUsers.Count > 0)
            {
                RemoveRange(lstFolderUsers);
            }
        }
    }
}

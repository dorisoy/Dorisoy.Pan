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
    public class PhysicalFolderUserRepository : GenericRepository<PhysicalFolderUser, DocumentContext>,
              IPhysicalFolderUserRepository
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        public PhysicalFolderUserRepository(
            IUnitOfWork<DocumentContext> uow,
            UserInfoToken userInfoToken,
            IPhysicalFolderRepository physicalFolderRepository)
            : base(uow)
        {
            _userInfoToken = userInfoToken;
            _physicalFolderRepository = physicalFolderRepository;
        }

        public void AddFolderUsers(Guid id, List<Guid> users)
        {
            var lstPFUser = new List<PhysicalFolderUser>();
            foreach (var userId in users)
            {
                if (!All.Where(c => c.FolderId == id && c.UserId == userId).Any())
                {
                    lstPFUser.Add(new PhysicalFolderUser { FolderId = id, UserId = userId });
                }
            }
            if (lstPFUser.Count() > 0)
            {
                AddRange(lstPFUser);
            }
        }
        public async Task AddPhysicalFolderUsersChildsById(Guid id, List<Guid> users)
        {
            this.AddFolderUsers(id, users);
            var physicalChildFolders = await _physicalFolderRepository.GetChildsHierarchyById(id);
            if (physicalChildFolders.Count() > 0)
            {
                foreach (var physicalChildFolder in physicalChildFolders)
                {
                    this.AddFolderUsers(physicalChildFolder.Id, users);
                }
            }
        }

        public void AssignPermission(Guid id, Guid folderId)
        {
            var data = All.Where(c => c.FolderId == id).Select(c => c.UserId).ToList();
            if (data.Any())
            {
                var phycalFolderUsers = data.Select(c => new PhysicalFolderUser
                {
                    FolderId = folderId,
                    UserId = c
                }).ToList();

                AddRange(phycalFolderUsers);
            }
            else
            {
                Add(new PhysicalFolderUser
                {
                    FolderId = folderId,
                    UserId = _userInfoToken.Id
                });
            }
        }

        public async Task RemovedFolderUsers(List<HierarchyFolder> lstFolders, Guid userId)
        {
            var lstFolderUsers = new List<PhysicalFolderUser>();
            foreach (var folder in lstFolders)
            {
                var folderUser = await All.Where(c => c.FolderId == folder.Id && c.UserId == userId).FirstOrDefaultAsync();
                if (folderUser != null)
                {
                    lstFolderUsers.Add(folderUser);
                }
            }
            if (lstFolderUsers.Count > 0)
            {
                RemoveRange(lstFolderUsers);
            }
        }
    }
}

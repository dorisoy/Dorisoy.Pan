using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository
{
    public class VirtualFolderRepository : GenericRepository<VirtualFolder, DocumentContext>,
          IVirtualFolderRepository
    {
        public VirtualFolderRepository(
            IUnitOfWork<DocumentContext> uow
            ) : base(uow)
        {

        }
        public async Task<VirtualFolder> GetRootFolder()
        {
            return await All.Where(c => c.ParentId == null).FirstOrDefaultAsync();
        }
        public async Task<VirtualFolder> SharedFolderExist(Guid PhysicalFolderId, Guid RootId)
        {
            return await All.Where(c => c.PhysicalFolderId == PhysicalFolderId && c.ParentId == RootId).FirstOrDefaultAsync();
        }
        public async Task<List<VirtualFolder>> GetVirualFoldersByPhysicalId(Guid PhysicalFolderId)
        {
            return await All.Where(c => c.PhysicalFolderId == PhysicalFolderId).ToListAsync();
        }

        public Guid AddVirtualFolder(Guid? parentId, Guid id, string name, Guid physicalFolderId)
        {
            var newId = Guid.NewGuid();
            var newFolder = new VirtualFolder
            {
                Id = newId,
                Name = name,
                ParentId = parentId,
                PhysicalFolderId = physicalFolderId
            };
            Add(newFolder);
            return newId;
        }

        public async Task<List<HierarchyFolder>> GetChildsHierarchyById(Guid id)
        {
            var idParam = new MySqlParameter("Id", id);
            return await _uow.Context.HierarchyFolders
                 .FromSqlRaw("CALL getVirtualFolderChildsHierarchyById(@Id)", idParam)
                 .ToListAsync();
        }

        public async Task<List<Guid>> GetFolderUserByPhysicalFolderId(Guid id)
        {
            var entities = await All.Include(c => c.VirtualFolderUsers)
                  .Where(c => c.PhysicalFolderId == id)
                  .Select(c => c.VirtualFolderUsers.Select(cs => cs.UserId))
                  .ToListAsync();
            return entities.SelectMany(c => c).ToList();
        }

        public async Task<List<HierarchyFolder>> GetParentsHierarchyById(Guid id)
        {
            var idParam = new MySqlParameter("Id", id);
            var folderHirarchy = await _uow.Context.HierarchyFolders
                 .FromSqlRaw("CALL getVirtualFolderParentsHierarchyById(@Id)", idParam)
                 .ToListAsync();
            return folderHirarchy.OrderByDescending(c => c.Level).ToList();
        }

        public async Task<List<HierarchyFolder>> GetParentsShared(Guid id)
        {
            var idParam = new MySqlParameter("Id", id);
            return await _uow.Context.HierarchyFolders
                 .FromSqlRaw("CALL getSharedParentsHierarchyById(@Id)", idParam)
                 .ToListAsync();

        }
        public async Task<List<HierarchyFolder>> GetChildsShared(Guid id)
        {
            var idParam = new MySqlParameter("Id", id);
            return await _uow.Context.HierarchyFolders
                 .FromSqlRaw("CALL getSharedChildsHierarchyById(@Id)", idParam)
                 .ToListAsync();
        }

        public async Task<string> GetParentFolderPath(Guid childId)
        {
            var parents = await GetParentsHierarchyById(childId);
            return string.Join("/", parents.Select(c => c.Name));
        }

        public async Task<string> GetFolderName(string name, Guid Id, int index, Guid userId)
        {
            var modifiedName = "";

            if (index != 0)
            {
                modifiedName = name + "(" + index + ")";
            }
            else
            {
                modifiedName = name;
            }
            var distinationSameNameFolder = await FindBy(c => c.ParentId == Id && c.Name == modifiedName && c.VirtualFolderUsers.Any(c => c.UserId == userId))
                                               .FirstOrDefaultAsync();
            if (distinationSameNameFolder != null)
            {
                return await GetFolderName(name, distinationSameNameFolder.ParentId.Value, ++index, userId);
            }
            else
            {
                return modifiedName;
            }
        }
    }
}

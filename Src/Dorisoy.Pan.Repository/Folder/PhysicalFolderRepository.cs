using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Dorisoy.Pan.Repository
{
    public class PhysicalFolderRepository : GenericRepository<PhysicalFolder, DocumentContext>,
          IPhysicalFolderRepository
    {
        public List<Guid> lstFolders { get; set; } = new List<Guid>();
        public PhysicalFolderRepository(
            IUnitOfWork<DocumentContext> uow
            ) : base(uow)
        {

        }
        public async Task<List<HierarchyFolder>> GetChildsHierarchyById(Guid id)
        {
            var idParam = new MySqlParameter("Id", id);
            return await _uow.Context.HierarchyFolders
                 .FromSqlRaw("CALL getPhysicalFolderChildsHierarchyById(@Id)", idParam)
                 .ToListAsync();
        }
        public async Task<List<HierarchyFolder>> GetParentsHierarchyById(Guid id)
        {
            var idParam = new MySqlParameter("Id", id);
            var folderHirarchy = await _uow.Context.HierarchyFolders
                 .FromSqlRaw("CALL getPhysicalFolderParentsHierarchyById(@Id)", idParam)
                 .ToListAsync();
            return folderHirarchy.OrderByDescending(c => c.Level).ToList();
        }
        public async Task<string> GetParentFolderPath(Guid childId)
        {
            var parents = await GetParentsHierarchyById(childId);
            return string.Join(Path.DirectorySeparatorChar, parents.Select(c => c.Name));
        }

        public async Task<string> GetParentOriginalFolderPath(Guid childId)
        {
            var parents = await GetParentsHierarchyById(childId);
            return string.Join(Path.DirectorySeparatorChar, parents.Select(c => c.Name));
        }
    }
}

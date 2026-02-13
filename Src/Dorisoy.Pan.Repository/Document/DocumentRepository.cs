using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository
{
    public class DocumentRepository : GenericRepository<Document, DocumentContext>,
          IDocumentRepository
    {
        public DocumentRepository(
            IUnitOfWork<DocumentContext> uow
            ) : base(uow)
        {
        }
        public async Task<List<Document>> GetDocumentsByPhysicalFolderId(Guid Id)
        {
            return await All.Where(c => c.PhysicalFolderId == Id).ToListAsync();
        }

        public async Task<string> GetDocumentName(string name, Guid physicalFolderId, int index, Guid userId)
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
            var distinationDocument = await FindBy(c => c.PhysicalFolderId == physicalFolderId && c.Name == modifiedName && c.CreatedBy== userId)
                                               .FirstOrDefaultAsync();
            if (distinationDocument != null)
            {
                return await GetDocumentName(name, distinationDocument.PhysicalFolderId, ++index, userId);
            }
            else
            {
                return modifiedName;
            }
        }
    }
}

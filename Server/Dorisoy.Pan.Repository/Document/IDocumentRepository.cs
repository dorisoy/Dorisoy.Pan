using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        Task<List<Document>> GetDocumentsByPhysicalFolderId(Guid Id);
        Task<string> GetDocumentName(string name, Guid physicalFolderId, int index, Guid userId);
    }
}

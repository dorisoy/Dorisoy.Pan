using Dorisoy.Pan.MediatR.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class DocumentUserPermissionCommandHandler : IRequestHandler<DocumentUserPermissionCommand, bool>
    {
        //private readonly IPhysicalFolderRepository _folderRepository;
        //private readonly IDocumentRepository _documentRepository;
        //private readonly IMapper _mapper;
        //private readonly IDocumentUserPermissionRepository _documentUserPermissionRepository;
        //private readonly IFolderUserPermissionRepository _folderUserPermissionRepository;
        //public DocumentUserPermissionCommandHandler(
        //    IPhysicalFolderRepository folderRepository,
        //    IMapper mapper,
        //    IDocumentRepository documentRepository,
        //    IDocumentUserPermissionRepository documentUserPermissionRepository,
        //    IFolderUserPermissionRepository folderUserPermissionRepository)
        //{
        //    _folderRepository = folderRepository;
        //    _mapper = mapper;
        //    _documentRepository = documentRepository;
        //    _documentUserPermissionRepository = documentUserPermissionRepository;
        //    _folderUserPermissionRepository = folderUserPermissionRepository;
        //}
        public async Task<bool> Handle(DocumentUserPermissionCommand request, CancellationToken cancellationToken)
        {
            //var document = await _documentRepository.FindAsync(request.Id);
            //var folders = await _folderRepository.GetParentsHierarchyById(document.FolderId);
            //List<DocumentUserPermission> lstUserPermission = new List<DocumentUserPermission>();
            //List<FolderUserPermission> lstFolderPermission = new List<FolderUserPermission>();
            //foreach (var user in request.Users)
            //{
            //    if (!_documentUserPermissionRepository.All.Any(c => c.DocumentId == document.Id && c.UserId == user))
            //    {
            //        lstUserPermission.Add(
            //              new DocumentUserPermission
            //              {
            //                  DocumentId = document.Id,
            //                  UserId = user,
            //              });
            //    }
            //    //foreach(var folder in folders)
            //    //{
            //    //    if (!_folderUserPermissionRepository.All.Any(c => c.FolderId == folder.Id && c.UserId == user))
            //    //    {
            //    //        lstUserPermission.Add(
            //    //              new FolderUserPermission
            //    //              {
            //    //                  //DocumentId = document.Id,
            //    //                  //UserId = user,
            //    //                  //IsDownload = true,
            //    //                  //IsWrite = true
            //    //              });
            //    //    }
            //    //}
            //}
            //if (lstUserPermission.Count() > 0)
            //{
            //    _documentUserPermissionRepository.AddRange(lstUserPermission);
            //}

            return true;
        }
    }
}

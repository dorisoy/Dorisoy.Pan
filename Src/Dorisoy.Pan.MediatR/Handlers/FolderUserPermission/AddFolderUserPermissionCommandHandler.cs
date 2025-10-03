using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class AddFolderUserPermissionCommandHandler : IRequestHandler<AddFolderUserPermissionCommand, ServiceResponse<bool>>
    {
        //private readonly IPhysicalFolderRepository _folderRepository;
        //private readonly ISharedFolderRepository _sharedFolderRepository;
        //private readonly IFolderUserPermissionRepository _folderUserPermissionRepository;
        //private readonly IMapper _mapper;
        //private readonly IUnitOfWork<DocumentContext> _uow;
        //public readonly UserInfoToken _userInfo;

        //public AddFolderUserPermissionCommandHandler(IPhysicalFolderRepository folderRepository,
        //    IMapper mapper,
        //    IUnitOfWork<DocumentContext> uow,
        //    UserInfoToken userInfo,
        //    IFolderUserPermissionRepository folderUserPermissionRepository,
        //    ISharedFolderRepository sharedFolderRepository)
        //{
        //    _folderRepository = folderRepository;
        //    _mapper = mapper;
        //    _uow = uow;
        //    _userInfo = userInfo;
        //    _folderUserPermissionRepository = folderUserPermissionRepository;
        //    _sharedFolderRepository = sharedFolderRepository;
        //}

        public async Task<ServiceResponse<bool>> Handle(AddFolderUserPermissionCommand request, CancellationToken cancellationToken)
        {
            //var currentFolder = await _folderRepository.FindAsync(request.FolderId);
            //List<FolderUserPermission> lstFolderUserPermission = new List<FolderUserPermission>();
            //List<SharedFolder> lstSharedFolder = new List<SharedFolder>();
            //var Id = Guid.NewGuid();
            //var sharedFolder = await _sharedFolderRepository.FindAsync(currentFolder.Id);
            //if (sharedFolder == null)
            //{
            //    sharedFolder = new SharedFolder
            //    {
            //        Id = Id,
            //        Name = currentFolder.Name,
            //        ParentId = null,
            //        SharedId = currentFolder.Id,
            //    };
            //    lstSharedFolder.Add(sharedFolder);
            //}

            //foreach (var user in request.Users)
            //{
            //    if (await _folderUserPermissionRepository.All.AnyAsync(c => c.FolderId == currentFolder.Id && c.UserId == user))
            //    {
            //        continue;
            //    }
            //    lstFolderUserPermission.Add(new FolderUserPermission { FolderId = currentFolder.Id, UserId = user });
            //}

            //var childFolders = await _folderRepository.GetChildsHierarchyById(currentFolder.Id);
            //if (childFolders.Count > 0)
            //{
            //    foreach (var heirarchy in childFolders)
            //    {
            //        if (!await _sharedFolderRepository.All.AnyAsync(c => c.Id == heirarchy.Id))
            //        {
            //            if (!lstSharedFolder.Any(c => c.Id == heirarchy.Id))
            //            {
            //                sharedFolder = new SharedFolder
            //                {
            //                    Id = heirarchy.Id,
            //                    Name = heirarchy.Name,
            //                    ParentId = heirarchy.Level == 0 ? Id : heirarchy.ParentId,
            //                    SharedId = currentFolder.Id,
            //                };
            //                lstSharedFolder.Add(sharedFolder);
            //            }
            //        }
            //    }
            //}
            //if (lstFolderUserPermission.Count() > 0)
            //{
            //    _folderUserPermissionRepository.AddRange(lstFolderUserPermission);
            //}
            //if (lstSharedFolder.Count() > 0)
            //{
            //    _sharedFolderRepository.AddRange(lstSharedFolder);
            //}
            //if (await _uow.SaveAsync() <= 0)
            //{
            //    return ServiceResponse<bool>.Return500();
            //}
            return await Task.FromResult(ServiceResponse<bool>.ReturnResultWith200(true));
        }
    }
}

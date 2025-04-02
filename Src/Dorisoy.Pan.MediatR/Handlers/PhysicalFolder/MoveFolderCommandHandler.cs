using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class MoveFolderCommandHandler : IRequestHandler<MoveFolderCommand, bool>
    {
        private readonly IPhysicalFolderUserRepository _physicalFolderUserRepository;
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly IDocumentRepository _documentRepository;
        public List<Document> lstDocument { get; set; } = new List<Document>();
        public MoveFolderCommandHandler(
            IPhysicalFolderUserRepository physicalFolderUserRepository,
            IVirtualFolderUserRepository virtualFolderUserRepository,
            IUnitOfWork<DocumentContext> uow,
            IVirtualFolderRepository virtualFolderRepository,
            IPhysicalFolderRepository physicalFolderRepository,
            UserInfoToken userInfoToken,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            IDocumentRepository documentRepository
            )
        {
            _physicalFolderUserRepository = physicalFolderUserRepository;
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _uow = uow;
            _virtualFolderRepository = virtualFolderRepository;
            _physicalFolderRepository = physicalFolderRepository;
            _userInfoToken = userInfoToken;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _documentRepository = documentRepository;
        }
        public async Task<bool> Handle(MoveFolderCommand request, CancellationToken cancellationToken)
        {
            var sourceFolder = await _virtualFolderRepository.FindAsync(request.SourceId);
            var sourceFolderChilds = await _virtualFolderRepository.GetChildsHierarchyById(sourceFolder.Id);
            var lstSharedUsers = new List<Guid>();

            var sourcePhysicalFolder = await _physicalFolderRepository.FindAsync(sourceFolder.PhysicalFolderId);
            var sourcePhysicalFolderChilds = await _physicalFolderRepository.GetChildsHierarchyById(sourcePhysicalFolder.Id);

            var distinationParentFolder = await _virtualFolderRepository.FindAsync(request.DistinationParentId);
            var distinationSharedParentFolder = await _virtualFolderRepository
                .FindBy(c => c.Id != distinationParentFolder.Id && c.PhysicalFolderId == distinationParentFolder.PhysicalFolderId)
                .FirstOrDefaultAsync();

            var distinationParentPhysicalFolder = await _physicalFolderRepository
                .All.Include(c => c.PhysicalFolderUsers.Where(u => u.UserId != _userInfoToken.Id))
                .Where(c => c.Id == distinationParentFolder.PhysicalFolderId)
                .FirstOrDefaultAsync();

            // var destinationPhysicalFolderPath = await _physicalFolderRepository.GetParentFolderPath(distinationParentFolder.PhysicalFolderId);
            var destinationPhysicalFolderPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, _userInfoToken.Id.ToString());

            if (!Directory.Exists(destinationPhysicalFolderPath))
            {
                Directory.CreateDirectory(destinationPhysicalFolderPath);
            }
            string name = await _virtualFolderRepository.GetFolderName(sourcePhysicalFolder.Name, distinationParentFolder.Id, 0, _userInfoToken.Id);
            //Change Physical Folder and Virtual Folder Parent Id
            sourcePhysicalFolder.ParentId = distinationParentPhysicalFolder.Id;
            sourcePhysicalFolder.Name = name;
            sourceFolder.ParentId = distinationParentFolder.Id;
            sourceFolder.Name = name;
            var documentList = await _documentRepository.GetDocumentsByPhysicalFolderId(sourcePhysicalFolder.Id);
            if (documentList.Count() > 0)
            {
                lstDocument.AddRange(documentList);
            }

            HierarchyFolder organoChild_ = new HierarchyFolder();
            organoChild_.Id = sourcePhysicalFolder.Id;
            organoChild_.Name = sourcePhysicalFolder.Name;
            organoChild_.ParentId = distinationParentFolder.Id;
            //organoChild_.Path = $"{ destinationPhysicalFolderPath}\\{sourcePhysicalFolder.SystemFolderName}";
            organoChild_.Path = destinationPhysicalFolderPath;
            var parentData = await PopulateChildrenDocument(organoChild_);

            //Add User to  Source Folder and Physical Folder From Destination Folder
            if (distinationParentPhysicalFolder.PhysicalFolderUsers.Count > 0)
            {
                lstSharedUsers = distinationParentPhysicalFolder.PhysicalFolderUsers.Select(c => c.UserId).ToList();
                List<PhysicalFolderUser> lstPhysicalFolders = new List<PhysicalFolderUser>();
                List<VirtualFolderUser> lstVirtualFolderFolders = new List<VirtualFolderUser>();
                foreach (var distinationFolderUser in distinationParentPhysicalFolder.PhysicalFolderUsers)
                {
                    lstPhysicalFolders.Add(
                        new PhysicalFolderUser
                        {
                            FolderId = sourcePhysicalFolder.Id,
                            UserId = distinationFolderUser.UserId
                        });

                    lstVirtualFolderFolders.Add(
                       new VirtualFolderUser
                       {
                           FolderId = sourceFolder.Id,
                           UserId = distinationFolderUser.UserId
                       });

                    if (sourceFolderChilds.Count > 0)
                    {
                        foreach (var sourcePhysicalFolderChild in sourcePhysicalFolderChilds)
                        {
                            lstPhysicalFolders.Add(
                           new PhysicalFolderUser
                           {
                               FolderId = sourcePhysicalFolderChild.Id,
                               UserId = distinationFolderUser.UserId
                           });
                        }
                        foreach (var sourceFolderChild in sourceFolderChilds)
                        {
                            lstVirtualFolderFolders.Add(
                           new VirtualFolderUser
                           {
                               FolderId = sourceFolderChild.Id,
                               UserId = distinationFolderUser.UserId
                           });
                        }
                    }
                }
                if (lstPhysicalFolders.Count > 0)
                {
                    _physicalFolderUserRepository.AddRange(lstPhysicalFolders);
                }
                if (lstVirtualFolderFolders.Count > 0)
                {
                    _virtualFolderUserRepository.AddRange(lstVirtualFolderFolders);
                }

                if (distinationSharedParentFolder != null)
                {
                    var hierarchyFolder = new HierarchyFolder
                    {
                        Id = sourceFolder.Id,
                        ParentId = sourceFolder.ParentId,
                        Level = 0,
                        Name = name,
                        PhysicalFolderId = sourceFolder.PhysicalFolderId,
                        Path = $"{destinationPhysicalFolderPath}\\{sourcePhysicalFolder.SystemFolderName}"
                    };
                    var newId = Guid.NewGuid();
                    var folderNew = new VirtualFolder
                    {
                        Id = newId,
                        Name = name,
                        ParentId = distinationSharedParentFolder.Id,
                        PhysicalFolderId = sourceFolder.PhysicalFolderId,
                        IsShared = false
                    };

                    lstSharedUsers.Add(_userInfoToken.Id);
                    var virtualFolderUsers = lstSharedUsers.Select(c => new VirtualFolderUser
                    {
                        FolderId = newId,
                        UserId = c
                    }).ToList();

                    folderNew.VirtualFolderUsers.AddRange(virtualFolderUsers);
                    var populatedOrgano = await PopulateChildren(hierarchyFolder, sourceFolderChilds, folderNew, lstSharedUsers);
                    _virtualFolderRepository.Add(folderNew);
                }

            }
            _physicalFolderRepository.Update(sourcePhysicalFolder);
            _virtualFolderRepository.Update(sourceFolder);
            // var fulldocumentPath = $"{_webHostEnvironment.ContentRootPath}\\{_pathHelper.DocumentPath}";
            var fulldocumentPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath);
            // var fullThumbdocumentPath = $"{_webHostEnvironment.WebRootPath}\\{_pathHelper.DocumentPath}";
            var fullThumbdocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, _pathHelper.DocumentPath);
            _documentRepository.UpdateRange(lstDocument);
            if (await _uow.SaveAsync() <= 0)
            {
                return false;
            }

            PopulateDocumentChildren(fulldocumentPath, fullThumbdocumentPath);
            _documentRepository.UpdateRange(lstDocument);
            if (await _uow.SaveAsync() <= 0)
            {
                return false;
            }
            return true;
        }

        private async Task<HierarchyFolder> PopulateChildren(HierarchyFolder sourceOrgano, ICollection<HierarchyFolder> organos, VirtualFolder root, List<Guid> users)
        {
            var children = organos.Where(x => x.ParentId == sourceOrgano.Id);
            foreach (var child in children)
            {
                var documentList = await _documentRepository.GetDocumentsByPhysicalFolderId(child.PhysicalFolderId);
                if (documentList.Count() > 0)
                {
                    lstDocument.AddRange(documentList);
                }

                //child
                HierarchyFolder organoChild_ = new HierarchyFolder();
                VirtualFolder virtualFolder_ = new VirtualFolder();
                virtualFolder_.Id = Guid.NewGuid();
                virtualFolder_.Name = child.Name;
                virtualFolder_.ParentId = root.Id;
                virtualFolder_.PhysicalFolderId = child.PhysicalFolderId;
                var virtualFolderUsers = users.Select(c => new VirtualFolderUser
                {
                    FolderId = virtualFolder_.Id,
                    UserId = c
                }).ToList();
                virtualFolder_.VirtualFolderUsers.AddRange(virtualFolderUsers);
                organoChild_.Id = child.Id;
                organoChild_.Name = child.Name;
                organoChild_.ParentId = sourceOrgano.Id;
                sourceOrgano.Children.Add(organoChild_);
                root.Children.Add(virtualFolder_);
                child.Path = sourceOrgano.Path;
                await PopulateChildren(child, organos, virtualFolder_, users);
            }
            return sourceOrgano;
        }

        private async Task<HierarchyFolder> PopulateChildrenDocument(HierarchyFolder sourceOrgano)
        {
            var children = await _physicalFolderRepository.All.Where(x => x.ParentId == sourceOrgano.Id).ToListAsync();
            foreach (var child in children)
            {

                var documentList = await _documentRepository.GetDocumentsByPhysicalFolderId(child.Id);
                if (documentList.Count() > 0)
                {
                    lstDocument.AddRange(documentList);
                }

                //child
                HierarchyFolder organoChild_ = new HierarchyFolder();
                organoChild_.Id = child.Id;
                organoChild_.Name = child.Name;
                organoChild_.ParentId = sourceOrgano.Id;
                organoChild_.Path = $"{ sourceOrgano.Path}\\{ child.SystemFolderName}";
                await PopulateChildrenDocument(organoChild_);
            }
            return sourceOrgano;
        }

        private void PopulateDocumentChildren(string rootPath, string thumbnailRootPath)
        {
            foreach (var document in lstDocument)
            {
                //var physicalPath = await _physicalFolderRepository.GetParentFolderPath(document.PhysicalFolderId);
                var originalPath = document.Path;
                var originalThumbPath = document.ThumbnailPath;
                var documentSystemName = document.Path.Split(Path.DirectorySeparatorChar).LastOrDefault();
                //document.Path = $"{physicalPath}\\{documentSystemName}";
                document.Path = Path.Combine(_userInfoToken.Id.ToString(), documentSystemName);
                //document.ThumbnailPath = ThumbnailHelper.IsSystemThumnails(document.ThumbnailPath)
                //    ? document.ThumbnailPath : $"{physicalPath}\\_thumbnail_{documentSystemName}";

                document.ThumbnailPath = ThumbnailHelper.IsSystemThumnails(document.ThumbnailPath)
                  ? document.ThumbnailPath : Path.Combine(_userInfoToken.Id.ToString(), "_thumbnail_" + documentSystemName);

                MoveFile(originalPath, document.Path, rootPath);
                if (!ThumbnailHelper.IsSystemThumnails(document.ThumbnailPath))
                {
                    MoveFile(originalThumbPath, document.ThumbnailPath, thumbnailRootPath);
                }
            }
        }

        private void MoveFile(string source, string destincation, string rootPath)
        {
            try
            {
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }
                //File.Move($"{rootPath}{source}", $"{rootPath}{destincation}");
                File.Move(Path.Combine(rootPath, source), Path.Combine(rootPath, destincation));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

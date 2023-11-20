using AutoMapper;
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
    public class CopyFolderCommandHandler : IRequestHandler<CopyFolderCommand, VirtualFolderDto>
    {
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        public List<Document> lstDocument { get; set; } = new List<Document>();
        public CopyFolderCommandHandler(
            IUnitOfWork<DocumentContext> uow,
            IVirtualFolderRepository virtualFolderRepository,
            IPhysicalFolderRepository physicalFolderRepository,
            UserInfoToken userInfoToken,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            IDocumentRepository documentRepository,
            IMapper mapper
            )
        {
            _uow = uow;
            _virtualFolderRepository = virtualFolderRepository;
            _physicalFolderRepository = physicalFolderRepository;
            _userInfoToken = userInfoToken;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _documentRepository = documentRepository;
            _mapper = mapper;
        }

        public async Task<VirtualFolderDto> Handle(CopyFolderCommand request, CancellationToken cancellationToken)
        {

            var sourceFolder = await _virtualFolderRepository.FindAsync(request.SourceId);
            var sourceFolderChilds = await _virtualFolderRepository.GetChildsHierarchyById(sourceFolder.Id);

            var sourcePhysicalFolder = await _physicalFolderRepository.FindAsync(sourceFolder.PhysicalFolderId);
            var sourcePhysicalFolderChilds = await _physicalFolderRepository.GetChildsHierarchyById(sourcePhysicalFolder.Id);

            var distinationParentFolder = await _virtualFolderRepository.FindAsync(request.DistinationParentId);
            var distinationSharedParentFolder = await _virtualFolderRepository
                .FindBy(c => c.Id != distinationParentFolder.Id && c.PhysicalFolderId == distinationParentFolder.PhysicalFolderId)
                .FirstOrDefaultAsync();

            var distinationParentPhysicalFolder = await _physicalFolderRepository
                .All.Include(c => c.PhysicalFolderUsers)
                .Where(c => c.Id == distinationParentFolder.PhysicalFolderId)
                .FirstOrDefaultAsync();

            string name = await _virtualFolderRepository.GetFolderName(sourcePhysicalFolder.Name, distinationParentFolder.Id, 0, _userInfoToken.Id);
            var users = distinationParentPhysicalFolder.PhysicalFolderUsers.Select(c => c.UserId).ToList();
            if (!users.Any())
            {
                users = new List<Guid> { _userInfoToken.Id };
            }

            var newPhysicalFolderId = Guid.NewGuid();
          //  var distinationFolderPathPath = await _physicalFolderRepository.GetParentFolderPath(distinationParentPhysicalFolder.Id);

            var newPhysicalFolder = new PhysicalFolder
            {
                Id = newPhysicalFolderId,
                Name = name,
                ParentId = distinationParentPhysicalFolder.Id,

            };
            var documentList = await _documentRepository.GetDocumentsByPhysicalFolderId(sourcePhysicalFolder.Id);
            if (documentList.Count() > 0)
            {
                foreach (var document in documentList)
                {
                    var documentId = Guid.NewGuid();
                    var documentSystemName = document.Path.Split(Path.DirectorySeparatorChar).LastOrDefault();

                    lstDocument.Add(new Document
                    {
                        Id = documentId,
                        PhysicalFolderId = newPhysicalFolderId,
                        Name = document.Name,
                        Extension = document.Extension,
                        OriginalPath = document.Path,
                        Path = Path.Combine(_userInfoToken.Id.ToString(), documentSystemName),
                        ThumbnailPath = ThumbnailHelper.IsSystemThumnails(document.ThumbnailPath)
                        ? document.ThumbnailPath : Path.Combine(_userInfoToken.Id.ToString(), "_thumbnail_" + documentSystemName),
                        OriginalThumbnailPath = document.ThumbnailPath,
                        Size = document.Size
                    });
                }
            }
            newPhysicalFolder.PhysicalFolderUsers.AddRange(users.Select(c => new PhysicalFolderUser
            {
                FolderId = newPhysicalFolderId,
                UserId = c
            }).ToList());

            var newVirtualFolderId1 = Guid.NewGuid();
            var newVirtaulFolder1 = new VirtualFolder
            {
                Id = newVirtualFolderId1,
                Name = name,
                ParentId = distinationParentFolder.Id,
                PhysicalFolderId = newPhysicalFolderId,
                IsShared = false
            };
            newVirtaulFolder1.VirtualFolderUsers.AddRange(users.Select(c => new VirtualFolderUser
            {
                FolderId = newVirtaulFolder1.Id,
                UserId = c
            }).ToList());

            VirtualFolder newVirtaulFolder2 = null;
            if (distinationSharedParentFolder != null)
            {
                var newVirtualFolderId2 = Guid.NewGuid();
                newVirtaulFolder2 = new VirtualFolder
                {
                    Id = newVirtualFolderId2,
                    Name = name,
                    ParentId = distinationSharedParentFolder.Id,
                    PhysicalFolderId = newPhysicalFolderId,
                    IsShared = false
                };
                newVirtaulFolder2.VirtualFolderUsers.AddRange(users.Select(c => new VirtualFolderUser
                {
                    FolderId = newVirtualFolderId2,
                    UserId = c
                }).ToList());
            }
            var hierarchyFolder = new HierarchyFolder
            {
                Id = sourceFolder.Id,
                ParentId = sourceFolder.ParentId,
                Level = 0,
                Name = name,
                PhysicalFolderId = sourceFolder.PhysicalFolderId,
                Path = ""
            };
            var populatedOrgano = await PopulateChildren(hierarchyFolder, sourceFolderChilds, newVirtaulFolder1, newVirtaulFolder2, users, newPhysicalFolder);
            _physicalFolderRepository.Add(newPhysicalFolder);
            _virtualFolderRepository.Add(newVirtaulFolder1);
            if (distinationSharedParentFolder != null)
            {
                _virtualFolderRepository.Add(newVirtaulFolder2);
            }

            if (lstDocument.Count > 0)
            {
                _documentRepository.AddRange(lstDocument);
            }
            if (await _uow.SaveAsync() <= 0)
            {
                return null;
            }

            //var containerPhysicalFolderPathRoot = await _physicalFolderRepository.GetParentFolderPath(distinationParentFolder.PhysicalFolderId);

            var fulldocumentPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath);

            var fullThumbdocumentPath = Path.Combine(_webHostEnvironment.WebRootPath, _pathHelper.DocumentPath);

            //var parentPath = $"{containerPhysicalFolderPathRoot}\\{newPhysicalFolder.SystemFolderName}";
            PopulateRootDocumentChildren(newPhysicalFolder, fulldocumentPath, fullThumbdocumentPath);
            PopulateDocumentChildren(newPhysicalFolder, fulldocumentPath, fullThumbdocumentPath);

            //if (lstDocument.Count > 0)
            //{
            //    _documentRepository.UpdateRange(lstDocument);
            //}

            //if (await _uow.SaveAsync() <= 0)
            //{
            //    return null;
            //}
            return _mapper.Map<VirtualFolderDto>(newVirtaulFolder1);

        }
        private async Task<HierarchyFolder> PopulateChildren(HierarchyFolder sourceOrgano, ICollection<HierarchyFolder> organos, VirtualFolder root, VirtualFolder root1, List<Guid> users, PhysicalFolder physicalRoot)
        {
            var children = organos.Where(x => x.ParentId == sourceOrgano.Id);
            foreach (var child in children)
            {
                //child
                HierarchyFolder organoChild_ = new HierarchyFolder();
                // Physical Folder Create
                PhysicalFolder physicalFolder_ = new PhysicalFolder();
                physicalFolder_.Id = Guid.NewGuid();
                physicalFolder_.Name = child.Name;
                physicalFolder_.ParentId = physicalRoot.Id;
                physicalFolder_.PhysicalFolderUsers.AddRange(users.Select(c => new PhysicalFolderUser
                {
                    FolderId = physicalFolder_.Id,
                    UserId = c
                }).ToList());

                var documentList = await _documentRepository.GetDocumentsByPhysicalFolderId(child.PhysicalFolderId);
                var currentPhysicalSys = _physicalFolderRepository.Find(child.PhysicalFolderId);
                if (documentList.Count() > 0)
                {
                    foreach (var document in documentList)
                    {
                       // var sourceFolderPathPath = await _physicalFolderRepository.GetParentFolderPath(child.PhysicalFolderId);
                        var documentSystemName = document.Path.Split(Path.DirectorySeparatorChar).LastOrDefault();
                        var documentId = Guid.NewGuid();
                        lstDocument.Add(new Document
                        {
                            Id = documentId,
                            PhysicalFolderId = physicalFolder_.Id,
                            Name = document.Name,
                            Extension = document.Extension,
                            Path = Path.Combine(_userInfoToken.Id.ToString(), documentSystemName),
                            OriginalPath = document.Path,
                            ThumbnailPath = ThumbnailHelper.IsSystemThumnails(document.ThumbnailPath)
                                            ? document.ThumbnailPath : Path.Combine(_userInfoToken.Id.ToString(), "_thumbnail_" + documentSystemName),
                            OriginalThumbnailPath = document.ThumbnailPath,
                            Size = document.Size
                        });
                    }
                }
                //Virtual Folder Create
                VirtualFolder virtualFolder_ = new VirtualFolder();
                virtualFolder_.Id = Guid.NewGuid();
                virtualFolder_.Name = child.Name;
                virtualFolder_.ParentId = root.Id;
                virtualFolder_.PhysicalFolderId = physicalFolder_.Id;
                virtualFolder_.VirtualFolderUsers.AddRange(users.Select(c => new VirtualFolderUser
                {
                    FolderId = virtualFolder_.Id,
                    UserId = c
                }).ToList());
                VirtualFolder virtualFolder1_ = null;
                if (root1 != null)
                {
                    virtualFolder1_ = new VirtualFolder();
                    virtualFolder1_.Id = Guid.NewGuid();
                    virtualFolder1_.Name = child.Name;
                    virtualFolder1_.ParentId = root1.Id;

                    virtualFolder1_.PhysicalFolderId = physicalFolder_.Id;
                    virtualFolder1_.VirtualFolderUsers.AddRange(users.Select(c => new VirtualFolderUser
                    {
                        FolderId = virtualFolder_.Id,
                        UserId = c
                    }).ToList());
                }
                organoChild_.Id = child.Id;
                organoChild_.Name = child.Name;
                organoChild_.ParentId = sourceOrgano.Id;
                organoChild_.PhysicalFolderId = child.PhysicalFolderId;
                organoChild_.Path = $"{sourceOrgano.Path}\\{currentPhysicalSys.SystemFolderName}";
                sourceOrgano.Children.Add(organoChild_);
                child.Path = $"{sourceOrgano.Path}\\{currentPhysicalSys.SystemFolderName}";
                root.Children.Add(virtualFolder_);
                physicalRoot.Children.Add(physicalFolder_);
                if (root1 != null)
                {
                    root1.Children.Add(virtualFolder1_);
                }
                await PopulateChildren(child, organos, virtualFolder_, virtualFolder1_, users, physicalFolder_);
            }
            return sourceOrgano;
        }

        private void PopulateRootDocumentChildren(PhysicalFolder physicalRoot, string rootPath, string thumbnailRootPath)
        {
            var path = Path.Combine(rootPath, _userInfoToken.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var thumbnailPath = Path.Combine(thumbnailRootPath, _userInfoToken.Id.ToString());
            if (!Directory.Exists(thumbnailPath))
            {
                Directory.CreateDirectory(thumbnailPath);
            }

            var documentList = lstDocument.Where(c => c.PhysicalFolderId == physicalRoot.Id).ToList();
            if (documentList.Count() > 0)
            {
                documentList.ForEach(c =>
                {
                    var originalPath = c.OriginalPath;
                    var originalThumbPath = c.OriginalThumbnailPath;
                    //var documentSystemName = c.Path.Split("\\").LastOrDefault();
                    //c.Path = $"{parentPath}\\{documentSystemName}";
                    //c.ThumbnailPath = ThumbnailHelper.IsSystemThumnails(c.ThumbnailPath) ? c.ThumbnailPath : $"{parentPath}\\_thumbnail_{documentSystemName}";
                    CopyFile(originalPath, c.Path, rootPath);
                    if (!ThumbnailHelper.IsSystemThumnails(c.ThumbnailPath))
                    {
                        CopyFile(originalThumbPath, c.ThumbnailPath, thumbnailRootPath);
                    }
                });
            }
        }

        private void PopulateDocumentChildren(PhysicalFolder physicalRoot, string rootPath, string thumbnailRootPath)
        {
            var path = Path.Combine(rootPath, _userInfoToken.Id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var thumbnailPath = Path.Combine(thumbnailRootPath, _userInfoToken.Id.ToString());
            if (!Directory.Exists(thumbnailPath))
            {
                Directory.CreateDirectory(thumbnailPath);
            }
            var children = physicalRoot.Children.ToList();
            foreach (var child in children)
            {
                //var folderPath = $"{parentPath}\\{child.SystemFolderName}";
                var documentList = lstDocument.Where(c => c.PhysicalFolderId == child.Id).ToList();
                if (documentList.Count() > 0)
                {

                    documentList.ForEach(c =>
                    {
                        var originalPath = c.OriginalPath;
                        var originalThumbPath = c.OriginalThumbnailPath;
                        // var documentSystemName = c.Path.Split("\\").LastOrDefault();
                        //c.Path = $"{parentPath}\\{child.SystemFolderName}\\{documentSystemName}";
                        //c.ThumbnailPath = ThumbnailHelper.IsSystemThumnails(c.ThumbnailPath) ? c.ThumbnailPath : $"{parentPath}\\{child.SystemFolderName}\\_thumbnail_{documentSystemName}";
                        CopyFile(originalPath, c.Path, rootPath);
                        if (!ThumbnailHelper.IsSystemThumnails(c.ThumbnailPath))
                        {
                            CopyFile(originalThumbPath, c.ThumbnailPath, thumbnailRootPath);
                        }
                    });
                }

                PopulateDocumentChildren(child, rootPath, thumbnailRootPath);
            }
        }

        private void CopyFile(string source, string destincation, string rootPath)
        {
            try
            {
                File.Copy(Path.Combine(rootPath, source), Path.Combine(rootPath, destincation));
            }
            catch (Exception ex)
            {

            }

        }
    }
}
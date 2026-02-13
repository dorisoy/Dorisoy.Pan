using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class DownloadFolderCommandHandler : IRequestHandler<DownloadFolderCommand, List<DownloadDocumentDto>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IPhysicalFolderUserRepository _physicalFolderUserRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfo;

        public DownloadFolderCommandHandler(IDocumentRepository documentRepository,
            IPhysicalFolderRepository physicalFolderRepository,
            IPhysicalFolderUserRepository physicalFolderUserRepository,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            UserInfoToken userInfo)
        {
            _documentRepository = documentRepository;
            _physicalFolderRepository = physicalFolderRepository;
            _physicalFolderUserRepository = physicalFolderUserRepository;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _userInfo = userInfo;
        }
        public async Task<List<DownloadDocumentDto>> Handle(DownloadFolderCommand request, CancellationToken cancellationToken)
        {
            var physicalFolder = await _physicalFolderRepository.All.FirstOrDefaultAsync(c => c.Id == request.Id);
            if (physicalFolder.ParentId.HasValue)
            {
                var childs = await _physicalFolderRepository.GetChildsHierarchyById(request.Id);
                var parentPath = await _physicalFolderRepository.GetParentFolderPath(request.Id);
                var parentOriginalPath = await _physicalFolderRepository.GetParentOriginalFolderPath(request.Id);
                parentPath = string.Join(Path.DirectorySeparatorChar, parentPath.Split(Path.DirectorySeparatorChar).SkipLast(1).ToList()) + Path.DirectorySeparatorChar;
                //parentPath = Path.Combine(parentPath.Split(Path.DirectorySeparatorChar).SkipLast(1).Select(c => c).ToArray());
                // parentOriginalPath = string.Join("\\", parentOriginalPath.Split("\\").SkipLast(1).ToList()) + "\\";
                var folderIdsToDownload = new List<Guid> { request.Id };
                folderIdsToDownload.AddRange(childs.Select(c => c.Id).ToList());
                var result = await _documentRepository.All.Where(c => EF.Constant(folderIdsToDownload).Contains(c.PhysicalFolderId))
                    .Select(c => new DownloadDocumentDto
                    {
                        Name = c.Name,
                        Path = c.Path,
                        Id = c.Id,
                        FolderId = c.PhysicalFolderId
                    }).ToListAsync();
                var uniquiFolderId = result.Select(c => c.FolderId).Distinct().ToList();
                Dictionary<Guid, string> folderPaths = new Dictionary<Guid, string>();

                foreach (var folderId in uniquiFolderId)
                {
                    var path = await _physicalFolderRepository.GetParentOriginalFolderPath(folderId);
                    folderPaths.Add(folderId, path);
                }

                result.ForEach(r =>
                {
                    folderPaths.TryGetValue(r.FolderId, out var folderPath);
                    r.FolderPath = r.Path.Replace(parentPath, "");
                    r.OriginalFolderPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, r.Path);
                    r.Path = Path.Combine(folderPath.Replace(parentOriginalPath, ""), r.Name);
                });
                return result;
            }
            else
            {
                var childs = await _physicalFolderRepository.GetChildsHierarchyById(request.Id);
                if (!physicalFolder.ParentId.HasValue)
                {
                    childs = childs.Where(cs => _physicalFolderUserRepository.All.Any(c => c.FolderId == cs.Id && c.UserId == _userInfo.Id)).ToList();
                }
                var parentPath = await _physicalFolderRepository.GetParentFolderPath(request.Id);
                var parentOriginalPath = await _physicalFolderRepository.GetParentOriginalFolderPath(request.Id);
                // parentPath = string.Join("\\", parentPath.Split("\\").SkipLast(1).ToList()) + "\\";
                //parentPath = Path.Combine(parentPath.Split(Path.DirectorySeparatorChar).SkipLast(1).Select(c => c).Reverse().ToArray());

                parentPath = string.Join(Path.DirectorySeparatorChar, parentPath.Split(Path.DirectorySeparatorChar).SkipLast(1).ToList()) + Path.DirectorySeparatorChar;
                //parentOriginalPath = Path.Combine(parentOriginalPath.Split(Path.DirectorySeparatorChar).SkipLast(1).Select(c => c).ToArray()) ;

                // No need to add Parent Folder.
                var folderIdsToDownload = new List<Guid> { };
                folderIdsToDownload.AddRange(childs.Select(c => c.Id).ToList());
                var result = await _documentRepository.All.Where(c => EF.Constant(folderIdsToDownload).Contains(c.PhysicalFolderId))
                    .Select(c => new DownloadDocumentDto
                    {
                        Name = c.Name,
                        Path = c.Path,
                        Id = c.Id,
                        FolderId = c.PhysicalFolderId
                    }).ToListAsync();

                var parentFoldersDocuments = await _documentRepository.All
                  .IgnoreQueryFilters()
                  .Where(c => c.PhysicalFolderId == request.Id
                      && c.CreatedBy == _userInfo.Id
                      && !c.DocumentDeleteds.Any(cs => cs.UserId == _userInfo.Id))
                  .Select(c => new DownloadDocumentDto
                  {
                      Name = c.Name,
                      Path = c.Path,
                      Id = c.Id,
                      FolderId = c.PhysicalFolderId
                  }).ToListAsync();

                result.AddRange(parentFoldersDocuments);
                var uniquiFolderId = result.Select(c => c.FolderId).Distinct().ToList();
                Dictionary<Guid, string> folderPaths = new Dictionary<Guid, string>();

                foreach (var folderId in uniquiFolderId)
                {
                    var path = await _physicalFolderRepository.GetParentOriginalFolderPath(folderId);
                    folderPaths.Add(folderId, path);
                }

                result.ForEach(r =>
                {
                    folderPaths.TryGetValue(r.FolderId, out var folderPath);
                    // r.OriginalFolderPath = $"{_webHostEnvironment.ContentRootPath}\\{_pathHelper.DocumentPath}{r.Path}";
                    r.OriginalFolderPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, r.Path);
                    r.Path = Path.Combine(folderPath, r.Name);
                });
                return result;
            }
        }
    }
}

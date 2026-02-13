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
    public class DownloadDocumentAndFolderCommandHandler
        : IRequestHandler<DownloadDocumentAndFolderCommand, List<DownloadDocumentDto>>
    {

        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;

        public DownloadDocumentAndFolderCommandHandler(IPhysicalFolderRepository physicalFolderRepository,
            IDocumentRepository documentRepository,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper)
        {
            _physicalFolderRepository = physicalFolderRepository;
            _documentRepository = documentRepository;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
        }
        public async Task<List<DownloadDocumentDto>> Handle(DownloadDocumentAndFolderCommand request, CancellationToken cancellationToken)
        {
            var result = new List<DownloadDocumentDto>();
            foreach (var id in request.FolderIds)
            {
                var childs = await _physicalFolderRepository.GetChildsHierarchyById(id);
                var parentpath = await _physicalFolderRepository.GetParentFolderPath(id);
                var parentOriginalPath = await _physicalFolderRepository.GetParentOriginalFolderPath(id);
                parentpath = string.Join(Path.DirectorySeparatorChar, parentpath.Split(Path.DirectorySeparatorChar).SkipLast(1).ToList()) + Path.DirectorySeparatorChar;
                parentOriginalPath = string.Join(Path.DirectorySeparatorChar, parentOriginalPath.Split(Path.DirectorySeparatorChar).SkipLast(1).ToList()) + Path.DirectorySeparatorChar;
                var folderIdsToDownload = new List<Guid> { id };
                folderIdsToDownload.AddRange(childs.Select(c => c.Id).ToList());
                var documentsToAdd = await _documentRepository.All.Where(c => EF.Constant(folderIdsToDownload).Contains(c.PhysicalFolderId))
                    .Select(c => new DownloadDocumentDto
                    {
                        Name = c.Name,
                        Path = c.Path,
                        Id = c.Id,
                        FolderId = c.PhysicalFolderId
                    }).ToListAsync();

                var uniquiFolderId = documentsToAdd.Select(c => c.FolderId).Distinct().ToList();
                Dictionary<Guid, string> folderPaths = new Dictionary<Guid, string>();

                foreach (var folderId in uniquiFolderId)
                {
                    var path = await _physicalFolderRepository.GetParentOriginalFolderPath(folderId);
                    folderPaths.Add(folderId, path);
                }

                documentsToAdd.ForEach(r =>
                {
                    folderPaths.TryGetValue(r.FolderId, out var folderPath);
                    r.FolderPath = r.Path.Replace(parentpath, "");
                    r.OriginalFolderPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, r.Path);
                    r.Path = Path.Combine(folderPath.Replace(parentOriginalPath, ""), r.Name);
                });
                result.AddRange(documentsToAdd);
            }

            var documents = await _documentRepository.All.Where(c => EF.Constant(request.DocumentIds).Contains(c.Id)).ToListAsync();
            foreach (var document in documents)
            {
                var path = await _physicalFolderRepository.GetParentOriginalFolderPath(document.PhysicalFolderId);
                result.Add(new DownloadDocumentDto
                {
                    Name = document.Name,
                    OriginalFolderPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, document.Path),
                    Id = document.Id,
                });
            }
            return result;
        }
    }
}

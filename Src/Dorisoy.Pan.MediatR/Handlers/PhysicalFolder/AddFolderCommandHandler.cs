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
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class AddFolderCommandHandler : IRequestHandler<AddFolderCommand, ServiceResponse<VirtualFolderInfoDto>>
    {
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IPhysicalFolderUserRepository _physicalFolderUserRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly ILogger<AddFolderCommandHandler> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConnectionMappingRepository _signalrService;

        public AddFolderCommandHandler(IPhysicalFolderRepository physicalFolderRepository,
            IVirtualFolderRepository virtualFolderRepository,
            IPhysicalFolderUserRepository physicalFolderUserRepository,
            IVirtualFolderUserRepository virtualFolderUserRepository,
            IMapper mapper,
            IUnitOfWork<DocumentContext> uow,
            ILogger<AddFolderCommandHandler> logger,
             IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            UserInfoToken userInfoToken,
            IConnectionMappingRepository signalrService
            )
        {
            _physicalFolderRepository = physicalFolderRepository;
            _virtualFolderRepository = virtualFolderRepository;
            _physicalFolderUserRepository = physicalFolderUserRepository;
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _mapper = mapper;
            _uow = uow;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
            _signalrService = signalrService;
        }
        public async Task<ServiceResponse<VirtualFolderInfoDto>> Handle(AddFolderCommand request, CancellationToken cancellationToken)
        {
            var existingEntityPermission = await _virtualFolderUserRepository
                .All
                .IgnoreQueryFilters()
                .Where(c => c.VirtualFolder.Name == request.Name
                    && c.VirtualFolder.ParentId == request.VirtualParentId
                    && c.UserId == _userInfoToken.Id && c.IsDeleted)
                .FirstOrDefaultAsync();

            // Existing Permission
            if (existingEntityPermission != null)
            {
                existingEntityPermission.IsDeleted = false;
                _virtualFolderUserRepository.Update(existingEntityPermission);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<VirtualFolderInfoDto>.Return500();
                }

                return await GetVirtualFolderInfoDto(existingEntityPermission.FolderId, true);
            }
            // Existing folder
            var existingEntity = await _physicalFolderRepository.All.Where(c => c.Name == request.Name
            && c.ParentId == request.PhysicalFolderId
            && c.PhysicalFolderUsers.Any(phu => phu.UserId == _userInfoToken.Id))
               .FirstOrDefaultAsync();
            if (existingEntity != null)
            {
                _logger.LogError("Folder is Already Exists.{0}", request);
                return ServiceResponse<VirtualFolderInfoDto>.ReturnFailed(409, "Folder is Already Exists.");
            }
            // Physical folder
            var physicalFolderId = Guid.NewGuid();
            var folder = new PhysicalFolder
            {
                Id = physicalFolderId,
                Name = request.Name,
                ParentId = request.PhysicalFolderId,
            };
            _physicalFolderRepository.Add(folder);
            _physicalFolderUserRepository.AssignPermission(request.PhysicalFolderId, physicalFolderId);
            var virtualFolderReturn = new VirtualFolder();
            var parentVirtualFolders = await _virtualFolderRepository.GetVirualFoldersByPhysicalId(request.PhysicalFolderId);
            foreach (var parentVirtualFolder in parentVirtualFolders)
            {
                var virtualFolder = _mapper.Map<VirtualFolder>(request);
                virtualFolder.Id = Guid.NewGuid();
                virtualFolder.PhysicalFolderId = physicalFolderId;
                virtualFolder.ParentId = parentVirtualFolder.Id;
                _virtualFolderRepository.Add(virtualFolder);
                _virtualFolderUserRepository.AssignPermission(request.VirtualParentId, virtualFolder.Id);
                if (parentVirtualFolder.Id == request.VirtualParentId)
                {
                    virtualFolderReturn = virtualFolder;
                }
            }
            if (await _uow.SaveAsync() <= 0)
            {
                return ServiceResponse<VirtualFolderInfoDto>.Return500();
            }
            // Create Directory
            //var folderPath = await _physicalFolderRepository.GetParentFolderPath(request.PhysicalFolderId);
            //var fullFolderPath = Path.Combine(_pathHelper.ContentRootPath,_pathHelper.DocumentPath,_userInfoToken.Id.ToString());
            //if (!Directory.Exists(fullFolderPath))
            //{
            //    Directory.CreateDirectory(fullFolderPath);
            //}
            return await GetVirtualFolderInfoDto(virtualFolderReturn.Id);
        }
        private async Task<ServiceResponse<VirtualFolderInfoDto>> GetVirtualFolderInfoDto(Guid id, bool isRestore = false)
        {
            var virtualFolderInfo = await _virtualFolderRepository.All
                .Include(c => c.PhysicalFolder)
                .ThenInclude(c => c.PhysicalFolderUsers)
                .ThenInclude(c => c.User)
                .Where(c => c.Id == id)
                .Select(c => new VirtualFolderInfoDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    PhysicalFolderId = c.PhysicalFolderId,
                    IsRestore = isRestore,
                    IsShared = c.IsShared,
                    Users = c.PhysicalFolder.PhysicalFolderUsers.Select(cs => new UserInfoDto
                    {
                        Email = cs.User.Email,
                        FirstName = cs.User.FirstName,
                        Id = cs.UserId,
                        LastName = cs.User.LastName
                    }).ToList()
                }).FirstOrDefaultAsync();
            return ServiceResponse<VirtualFolderInfoDto>.ReturnResultWith201(virtualFolderInfo);

        }
    }
}

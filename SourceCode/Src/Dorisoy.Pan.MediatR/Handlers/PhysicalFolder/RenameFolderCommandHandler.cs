using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class RenameFolderCommandHandler
        : IRequestHandler<RenameFolderCommand, ServiceResponse<bool>>
    {
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;

        public RenameFolderCommandHandler(IPhysicalFolderRepository physicalFolderRepository,
            IVirtualFolderRepository virtualFolderRepository,
            IUnitOfWork<DocumentContext> uow)
        {
            _physicalFolderRepository = physicalFolderRepository;
            _virtualFolderRepository = virtualFolderRepository;
            _uow = uow;
        }
        public async Task<ServiceResponse<bool>> Handle(RenameFolderCommand request, CancellationToken cancellationToken)
        {
            var physicalFolder = await _physicalFolderRepository.FindAsync(request.Id);
            if (physicalFolder == null)
            {
                return ServiceResponse<bool>.Return404();
            }

            var virtualFolders = await _virtualFolderRepository.All
                .Where(c => c.PhysicalFolderId == request.Id)
                .ToListAsync();
            physicalFolder.Name = request.Name;
            _physicalFolderRepository.Update(physicalFolder);

            virtualFolders.ForEach(folder =>
            {
                folder.Name = request.Name;
                _virtualFolderRepository.Update(folder);
            });

            if (await _uow.SaveAsync() <= 0)
            {
                return ServiceResponse<bool>.Return500();
            }
            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}

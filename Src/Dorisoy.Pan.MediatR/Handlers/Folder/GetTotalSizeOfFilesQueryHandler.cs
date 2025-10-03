using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers.Folder
{
    public class GetTotalSizeOfFilesQueryHandler : IRequestHandler<GetTotalSizeOfFilesQuery, long>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDocumentRepository _documentRepository;

        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        public GetTotalSizeOfFilesQueryHandler(
             IWebHostEnvironment webHostEnvironment,
             IDocumentRepository documentRepository,
              PathHelper pathHelper,
              UserInfoToken userInfoToken)
        {
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
            _documentRepository = documentRepository;
        }

        public async Task<long> Handle(GetTotalSizeOfFilesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var folder = await _documentRepository.All
                    .Where(p=>p.CreatedBy==_userInfoToken.Id && !p.IsDeleted)
                    .SumAsync(p=>p.Size);

                return folder;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}

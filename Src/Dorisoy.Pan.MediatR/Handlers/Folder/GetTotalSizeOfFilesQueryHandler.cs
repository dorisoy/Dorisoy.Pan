using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers.Folder
{
    public class GetTotalSizeOfFilesQueryHandler : IRequestHandler<GetTotalSizeOfFilesQuery, long>
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        public GetTotalSizeOfFilesQueryHandler(
             IWebHostEnvironment webHostEnvironment,
              PathHelper pathHelper,
              UserInfoToken userInfoToken)
        {
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        public Task<long> Handle(GetTotalSizeOfFilesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var path = Path.Combine(_pathHelper.ContentRootPath, 
                    _pathHelper.DocumentPath, 
                    _userInfoToken.Id.ToString());

                var dInfo = new DirectoryInfo(path);

                if (!dInfo.Exists)
                    return Task.FromResult<long>(0);

                var size = DirectorySizeCalculation.DirectorySize(dInfo, true);

                return Task.FromResult(size);
            }
            catch (Exception)
            {
                return Task.FromResult<long>(0);
            }
        }
    }
}

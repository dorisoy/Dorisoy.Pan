using MediatR;
using Microsoft.AspNetCore.Hosting;

using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Helper;
using System.IO;
using System.Linq;
using Dorisoy.Pan.Data.Dto;

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
        public async Task<long> Handle(GetTotalSizeOfFilesQuery request, CancellationToken cancellationToken)
        {
            var path = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, _userInfoToken.Id.ToString());
            DirectoryInfo dInfo = new DirectoryInfo(path);
            var size = DirectorySizeCalculation.DirectorySize(dInfo, true);
            return size;
        }
     
    }
}

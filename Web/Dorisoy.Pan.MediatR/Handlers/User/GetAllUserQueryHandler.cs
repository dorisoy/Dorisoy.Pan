using AutoMapper;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Dorisoy.Pan.Helper;
using System;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, List<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;

        public GetAllUserQueryHandler(
           IUserRepository userRepository,
            IMapper mapper,
            PathHelper pathHelper,
            UserInfoToken userInfoToken)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        public async Task<List<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var entities = await _userRepository.All.ToListAsync();

            var entitiesDto = _mapper.Map<List<UserDto>>(entities);

            //entitiesDto.ForEach(e =>
            //{
            //    e.Size = getSize(e.Id);
            //});
            return entitiesDto;
            //Size
        }
        public long getSize(Guid userId)
        {
            var path = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, userId.ToString());
            DirectoryInfo dInfo = new DirectoryInfo(path);
            var size = DirectorySizeCalculation.DirectorySize(dInfo, true);
            return size;
        }
    }
}

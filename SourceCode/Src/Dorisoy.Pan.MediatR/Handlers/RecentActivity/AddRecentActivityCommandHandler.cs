using AutoMapper;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class AddRecentActivityCommandHandler : IRequestHandler<AddRecentActivityCommand, bool>
    {
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IRecentActivityRepository _recentActivityRepository;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        public AddRecentActivityCommandHandler(
            IUnitOfWork<DocumentContext> uow,
            IRecentActivityRepository recentActivityRepository,
            IMapper mapper,
             UserInfoToken userInfoToken
            )
        {
            _uow = uow;
            _recentActivityRepository = recentActivityRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
        }
        public async Task<bool> Handle(AddRecentActivityCommand request, CancellationToken cancellationToken)
        {
            RecentActivity existingEntity = null;
            if (request.DocumentId != null && request.Action == RecentActivityType.VIEWED)
            {

                existingEntity = await _recentActivityRepository.All
                    .Where(c => c.DocumentId == request.DocumentId && c.Action == request.Action)
                    .FirstOrDefaultAsync();
            }
            if (existingEntity != null)
            {
                existingEntity.CreatedDate = DateTime.Now;
                _recentActivityRepository.Update(existingEntity);
            } 
            else
            {
                existingEntity = _mapper.Map<RecentActivity>(request);
                existingEntity.UserId = _userInfoToken.Id;
                existingEntity.CreatedDate = DateTime.Now;
                _recentActivityRepository.Add(existingEntity);
            }
            if (await _uow.SaveAsync() <= 0)
            {
                return false;
            }
            return true;
        }
    }
}

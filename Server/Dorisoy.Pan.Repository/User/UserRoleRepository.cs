using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Domain;

namespace Dorisoy.Pan.Repository
{

	public interface IUserRoleRepository : IGenericRepository<UserRole>
	{

	}


	public class UserRoleRepository : GenericRepository<UserRole, DocumentContext>, IUserRoleRepository
	{
        public UserRoleRepository(
            IUnitOfWork<DocumentContext> uow) : base(uow)
        {}

    }

	public interface IUserRoleClaimRepository : IGenericRepository<RoleClaim>
	{

	}

	public class UserRoleClaimRepository : GenericRepository<RoleClaim, DocumentContext>, IUserRoleClaimRepository
	{
		public UserRoleClaimRepository(
			IUnitOfWork<DocumentContext> uow) : base(uow)
		{ }

	}
}

using Dorisoy.Pan.Data;
using Microsoft.EntityFrameworkCore;

namespace Dorisoy.Pan.Common
{
   public class StateHelpers
    {
        public static EntityState ConvertState(ObjectState objstate)
        {
            switch (objstate)
            {
                case ObjectState.Added:
                    return EntityState.Added;
                case ObjectState.Modified:
                    return EntityState.Modified;
                case ObjectState.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }
    }
}

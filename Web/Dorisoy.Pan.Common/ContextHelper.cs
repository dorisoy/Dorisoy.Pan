using Dorisoy.Pan.Data;
using Microsoft.EntityFrameworkCore;

namespace Dorisoy.Pan.Common
{
    public static class ContextHelper
    {
        public static void ApplyStateChanges(this DbContext context )
        {
            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                BaseEntity stateInfo = entry.Entity;
                if(stateInfo.ObjectState== ObjectState.Modified)
                {
                    //entry.Entity.ModifiedBy = UserInfo.UserName;
                    entry.Entity.ModifiedDate = System.DateTime.UtcNow;
                }
                else if(stateInfo.ObjectState == ObjectState.Added)
                {
                    //entry.Entity.CreatedBy = UserInfo.UserName;
                }
                else if (stateInfo.ObjectState == ObjectState.Deleted)
                {
                    //entry.Entity.DeletedBy = UserInfo.UserName;
                    entry.Entity.DeletedDate = System.DateTime.UtcNow;
                }
                entry.State = StateHelpers.ConvertState(stateInfo.ObjectState);
            }
        }
    }
}

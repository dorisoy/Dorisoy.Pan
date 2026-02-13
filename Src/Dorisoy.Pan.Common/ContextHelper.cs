using Dorisoy.Pan.Data;
using Microsoft.EntityFrameworkCore;

namespace Dorisoy.Pan.Common
{
    public static class ContextHelper
    {
        /// <summary>
        /// 应用状态更改到上下文中的实体。根据实体的ObjectState属性，
        /// 设置相应的CreatedDate、ModifiedDate和DeletedDate，并将实体的状态转换为EntityState。
        /// </summary>
        /// <param name="context"></param>
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

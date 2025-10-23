using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
	/// <summary>
	/// 角色权限
	/// </summary>
	public class RoleClaim : IdentityRoleClaim<Guid>
    {
        public string Description { get; set; } = "";
        public string Group { get; set; } = "";

        public virtual Role Role { get; set; }

        public RoleClaim() : base() { }

        public RoleClaim(string roleClaimDescription = null, 
            string roleClaimGroup = null) : base()
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }


        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DeletedDate { get; set; }
        public Guid? DeletedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}

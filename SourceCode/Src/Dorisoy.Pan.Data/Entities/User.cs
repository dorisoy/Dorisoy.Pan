using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string ProfilePhoto { get; set; }
        public string Provider { get; set; }
        public string Address { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeletedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public ICollection<PhysicalFolderUser> Folders { get; set; } = new List<PhysicalFolderUser>();
        public bool IsAdmin { get; set; }
    }
}

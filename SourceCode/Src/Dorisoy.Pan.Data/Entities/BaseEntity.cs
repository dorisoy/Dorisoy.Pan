using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.Pan.Data
{
    public abstract class BaseEntity
    {
        private DateTime _createdDate;
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate
        {
            get => _createdDate.ToLocalTime();
            set => _createdDate = value.ToLocalTime();
        }
        public Guid CreatedBy { get; set; }

        private DateTime _modifiedDate;
        [Column(TypeName= "datetime")]
        public DateTime ModifiedDate
        {
            get => _modifiedDate.ToLocalTime();
            set => _modifiedDate = value.ToLocalTime();
        }
        public Guid ModifiedBy { get; set; }
        private DateTime? _deletedDate;
        [Column(TypeName = "datetime")]
        public DateTime? DeletedDate
        {
            get => _deletedDate?.ToLocalTime();
            set => _deletedDate = value?.ToLocalTime();
        }
        public Guid? DeletedBy { get; set; }
        [NotMapped]
        public ObjectState ObjectState { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

using System;

namespace Dorisoy.Pan.Data
{
    /// <summary>
    /// 审计信息
    /// </summary>
    public class Audit : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } 
        public string TableName { get; set; } 
        public DateTime DateTime { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; } 
        public string AffectedColumns { get; set; }
        public string PrimaryKey { get; set; } 
    }
}
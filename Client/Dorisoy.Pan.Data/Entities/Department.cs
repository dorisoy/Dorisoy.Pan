using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorisoy.PanClient.Data
{
    /// <summary>
    /// 部门
    /// </summary>
    public class Department : BaseEntity
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 唯一编码
        /// </summary>
        [MaxLength(200)]
        public string Code { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath { get; set; }

        public virtual ICollection<Department> Children { get; set; } = new List<Department>();

    }
}

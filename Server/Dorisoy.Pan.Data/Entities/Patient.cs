using System;

namespace Dorisoy.Pan.Data;

/// <summary>
/// 患者信息
/// </summary>
public class Patient : BaseEntity
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string PhoneNumber { get; set; }
    public string RaleName { get; set; }
    public Sex Sex { get; set; }
    public string Address { get; set; }
    public string StorePath { get; set; }
    public Guid? VirtualFolderId { get; set; }
    public Guid? PhysicalFolderId { get; set; }
}

namespace Dorisoy.Pan.Models;

public class PatientModel : BaseModel
{
    [Reactive] public Guid Id { get; set; }
    [Reactive] public string Code { get; set; }
    [Reactive] public string PhoneNumber { get; set; }
    [Reactive] public string RaleName { get; set; }
    [Reactive] public Sex Sex { get; set; }
    [Reactive] public string Address { get; set; }
    [Reactive] public ObservableCollection<DocumentModel> Videos { get; set; } = [];
    [Reactive] public ObservableCollection<DocumentModel> Images { get; set; } = [];
    [Reactive] public string StorePath { get; set; }
    [Reactive] public string CreateBy { get; set; }
    [Reactive] public bool Selected { get; set; }

    /// <summary>
    /// 当前项目的虚拟目录
    /// </summary>
    [Reactive] public Guid VirtualFolderId { get; set; }

    /// <summary>
    /// 当前项目的物理目录
    /// </summary>
    [Reactive] public Guid PhysicalFolderId { get; set; }

    /// <summary>
    /// 当前项目的虚拟缩略目录
    /// </summary>
    [Reactive] public Guid VirtualThumbnailFolderId { get; set; }

    /// <summary>
    /// 当前项目的物理缩略目录
    /// </summary>
    [Reactive] public Guid PhysicalThumbnailFolderId { get; set; }
}



public enum StretchModel
{
    /// <summary>
    /// 内容保留其原始大小
    /// </summary>
    [Description("原始大小")]
    None,

    /// <summary>
    /// 调整内容的大小以填充目标维度。纵横比未保留
    /// </summary>
    [Description("填充目标")]
    Fill,

    /// <summary>
    /// 调整内容的大小以适应目标维度，同时保留其原生纵横比
    /// </summary>
    [Description("纵横比大小自适应")]
    Uniform,

    /// <summary>
    /// 调整内容的大小以完全填充目标矩形，同时保留其原始纵横比。如果内容的纵横比与所分配空间的纵横比不匹配，则部分内容可能不可见
    /// </summary>
    [Description("纵横比完全填充")]
    UniformToFill
}

namespace Dorisoy.PanClient.Models;

public class UploadingModel : ReactiveObject
{
    [Reactive] public double Value { get; set; }
    [Reactive] public string ValueFormat { get; set; }
    [Reactive] public string FileName { get; set; }
}

public class DocumentModel : BaseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid PhysicalFolderId { get; set; }
    public string Extension { get; set; }
    public string Path { get; set; }
    public long Size { get; set; }
    public string ThumbnailPath { get; set; }
    public Symbol ThumbnailIcon { get; set; } = Symbol.OpenFolderFilled;
    public Guid PatienterId { get; set; }
    public string OriginalPath { get; set; }
    public string OriginalThumbnailPath { get; set; }
    public List<UserModel> PhysicalUsers { get; set; } = new();

    public int Comments { get; set; }
    public List<DocumentComment> DocComments { get; set; } = new();
    public bool IsAttachment { get; set; }
    public DocType DocType { get; set; }
    public FileType FileType { get; set; }

    public ReactiveCommand<DocumentModel, Unit> PlayCommand { get; set; }

    [Reactive] public Bitmap Cover { get; set; }
    [Reactive] public bool Selected { get; set; }

    public string PathURL { get; set; }
}

/// <summary>
/// 文档类型
/// </summary>
public enum DocType : int
{
    File = 1,
    Folder = 2
}

public enum FileType : int
{
    Image = 1,
    Video = 2
}

public class DocumentFolderModel : BaseModel
{
    public Guid Id { get; set; }

    [Reactive] public string Name { get; set; }
    [Reactive] public Guid PhysicalFolderId { get; set; }
    [Reactive] public string Extension { get; set; }
    [Reactive] public string Path { get; set; }
    [Reactive] public long Size { get; set; }
    [Reactive] public string ThumbnailPath { get; set; }
    [Reactive] public Symbol ThumbnailIcon { get; set; } = Symbol.OpenFolderFilled;
    [Reactive] public Guid PatienterId { get; set; }
    [Reactive] public string OriginalPath { get; set; }
    [Reactive] public string OriginalThumbnailPath { get; set; }
    [Reactive] public Bitmap Cover { get; set; }
    [Reactive] public List<UserModel> PhysicalUsers { get; set; } = new List<UserModel>();

    //文件夹相关
    [Reactive] public Guid VirtualFolderId { get; set; }
    [Reactive] public Guid? ParentId { get; set; }
    [Reactive] public bool IsShared { get; set; }
    [Reactive] public bool IsRestore { get; set; }
    [Reactive] public bool IsStarred { get; set; }
    [Reactive] public List<UserModel> Users { get; set; }

    [Reactive] public DocType DocType { get; set; }
    [Reactive] public bool Selected { get; set; }

    public List<Guid> DeletedUserIds { get; set; }

}


public class UploadDocumentCommand
{
    public Guid FolderId { get; set; }
    public DocumentModel Documents { get; set; }
    public string FullPath { get; set; }
}

public class UploadDocumentsCommand
{
    public Guid FolderId { get; set; }
    public DocumentModel Documents { get; set; }
}

public class TraversingModel
{
    public string RootPath { get; set; }
    public List<string> FolderPath { get; set; } = new();
    public List<string> FilePath { get; set; } = new();
}


public class DeletedDocumentInfoModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ThumbnailPath { get; set; }
    public DateTime? DeletedDate { get; set; }
}

public class DocumentCommentModel
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid DocumentId { get; set; }
    public string Comment { get; set; }
    public string UserName { get; set; }
}


public class UploadFile
{
    public Guid Id { get; set; }
    /// <summary>
    ///是否含有缩略
    /// </summary>
    public bool IsThumbnail { get; set; }

    public string Path { get; set; }
    /// <summary>
    /// 文件对应的虚拟目录
    /// </summary>
    public Guid VirtualFolderId { get; set; }

    /// <summary>
    /// 文件对应的物理目录
    /// </summary>
    public Guid PhysicalFolderId { get; set; }

    /// <summary>
    /// 文件对应的虚拟缩略目录
    /// </summary>
    public Guid VirtualThumbnailFolderId { get; set; }

    /// <summary>
    /// 文件对应的物理缩略目录
    /// </summary>
    public Guid PhysicalThumbnailFolderId { get; set; }

    public static UploadFile Create(PatientModel? patient, string path, bool isThumbnail = true)
    {
        return new UploadFile()
        {
            Id = Guid.NewGuid(),
            Path = path,
            IsThumbnail = isThumbnail,
            PhysicalFolderId = patient?.PhysicalFolderId ?? Guid.Empty,
            VirtualFolderId = patient?.VirtualFolderId ?? Guid.Empty,
            PhysicalThumbnailFolderId = patient?.PhysicalThumbnailFolderId ?? Guid.Empty,
            VirtualThumbnailFolderId = patient?.VirtualThumbnailFolderId ?? Guid.Empty,
        };
    }

}

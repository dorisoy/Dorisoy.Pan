namespace Dorisoy.PanClient.Models;

public class VirtualFolderInfoModel : ReactiveObject
{
    [Reactive] public Guid Id { get; set; }
    [Reactive] public string Name { get; set; }
    [Reactive] public Guid? ParentId { get; set; }
    [Reactive] public Guid PhysicalFolderId { get; set; }
    [Reactive] public bool IsShared { get; set; }
    [Reactive] public bool IsRestore { get; set; }
    [Reactive] public bool IsStarred { get; set; }
    [Reactive] public DateTime CreatedDate { get; set; }
    [Reactive] public List<UserModel> Users { get; set; }
    public Symbol ThumbnailIcon { get; set; } = Symbol.FolderFilled;
}

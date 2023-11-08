namespace Dorisoy.PanClient.Models;

public class VirtualFolderModel : ReactiveObject
{
    [Reactive] public Guid Id { get; set; }
    [Reactive] public string Name { get; set; }
    [Reactive] public Guid? ParentId { get; set; }
    public VirtualFolderModel Parent { get; set; }
    [Reactive] public string Size { get; set; }
    [Reactive] public Guid PhysicalFolderId { get; set; }
    [Reactive] public bool IsShared { get; set; }
    [Reactive] public PhysicalFolderModel PhysicalFolder { get; set; }
    [Reactive] public ICollection<VirtualFolderModel> Children { get; set; } = new List<VirtualFolderModel>();
    [Reactive] public ICollection<VirtualFolderUserModel> VirtualFolderUsers { get; set; } = new List<VirtualFolderUserModel>();
}

public class AddFolderModel : ReactiveObject
{
    [Reactive] public string Name { get; set; }
    [Reactive] public Guid VirtualParentId { get; set; }
    [Reactive] public Guid PhysicalFolderId { get; set; }
}

namespace Dorisoy.PanClient.Commands;

public class AddFolderCommand
{
    public string Name { get; set; }
    public Guid VirtualParentId { get; set; }
    public Guid PhysicalFolderId { get; set; }
}

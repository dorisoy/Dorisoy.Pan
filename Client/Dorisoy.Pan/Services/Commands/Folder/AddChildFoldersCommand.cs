namespace Dorisoy.PanClient.Commands;

public class AddChildFoldersCommand
{
    public Guid VirtualFolderId { get; set; }
    public Guid PhysicalFolderId { get; set; }
    public List<string> Paths { get; set; }
}

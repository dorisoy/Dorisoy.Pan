namespace Dorisoy.PanClient.Commands;

public class RemoveFolderRightForUserCommand
{
    public Guid FolderId { get; set; }
    public Guid PhysicalFolderId { get; set; }
    public Guid UserId { get; set; }
}

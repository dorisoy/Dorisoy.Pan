namespace Dorisoy.PanClient.Models;

public class PhysicalFolderUserDto
{
    public Guid FolderId { get; set; }
    public Guid UserId { get; set; }
    public PhysicalFolderDto PhysicalFolder { get; set; }
    public UserDto User { get; set; }
}

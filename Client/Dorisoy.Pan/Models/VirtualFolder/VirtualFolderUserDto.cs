namespace Dorisoy.Pan.Models;

public class VirtualFolderUserDto
{
    public Guid FolderId { get; set; }
    public Guid UserId { get; set; }
    public VirtualFolderDto VirtualFolder { get; set; }
    public UserDto User { get; set; }
}

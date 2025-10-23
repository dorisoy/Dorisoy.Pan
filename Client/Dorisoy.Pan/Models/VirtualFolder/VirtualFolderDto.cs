namespace Dorisoy.Pan.Models;

public class VirtualFolderDto : BaseDto
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public VirtualFolderDto Parent { get; set; }
    public string Size { get; set; }
    public Guid PhysicalFolderId { get; set; }
    public bool IsShared { get; set; }
    public PhysicalFolderDto PhysicalFolder { get; set; }
    public ICollection<VirtualFolderDto> Children { get; set; } = new List<VirtualFolderDto>();
    public ICollection<VirtualFolderUserDto> VirtualFolderUsers { get; set; } = new List<VirtualFolderUserDto>();
}

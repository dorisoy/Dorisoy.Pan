namespace Dorisoy.PanClient.Models;

public class PhysicalFolderDto : BaseDto
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public PhysicalFolderDto Parent { get; set; }
    public string Size { get; set; }
    public ICollection<PhysicalFolderDto> Children { get; set; } = new List<PhysicalFolderDto>();
    public ICollection<PhysicalFolderUserDto> PhysicalFolderUsers { get; set; } = new List<PhysicalFolderUserDto>();
}

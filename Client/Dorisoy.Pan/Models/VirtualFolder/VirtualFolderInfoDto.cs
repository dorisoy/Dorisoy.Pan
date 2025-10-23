namespace Dorisoy.Pan.Models;

public class VirtualFolderInfoDto : BaseDto
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public Guid PhysicalFolderId { get; set; }
    public bool IsShared { get; set; }
    public bool IsRestore { get; set; }
    public bool IsStarred { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<UserInfoDto> Users { get; set; }
}

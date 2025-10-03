namespace Dorisoy.PanClient.Models;

public class DeletedVirtualFolderDto : BaseDto
{
    public string Name { get; set; }
    public bool IsShared { get; set; }
    public DateTime? DeletedDate { get; set; }
}

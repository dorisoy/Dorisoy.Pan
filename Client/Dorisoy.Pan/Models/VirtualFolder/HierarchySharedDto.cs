namespace Dorisoy.PanClient.Models;

public class HierarchySharedDto : BaseDto
{
    public bool IsParentShared { get; set; } = false;
    public bool IsChildShared { get; set; } = false;

}

namespace Dorisoy.PanClient.Commands;

public class NotificationCommand
{
    public List<Guid> Users { get; set; } = new List<Guid>();
    public Guid FolderId { get; set; }
}

namespace Dorisoy.PanClient.Models;

public class ActionModel : BaseModel
{
    public Guid Id { get; set; }
    [Reactive] public string Name { get; set; }
    [Reactive] public string Title { get; set; }
    public Guid PageId { get; set; }
    [Reactive] public string Code { get; set; }
    [Reactive] public int Order { get; set; }
}

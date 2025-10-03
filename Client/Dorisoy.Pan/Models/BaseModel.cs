namespace Dorisoy.PanClient.Models;


public sealed class PreserveAttribute : Attribute
{
    public bool AllMembers;
    public bool Conditional;
    public PreserveAttribute(bool allMembers, bool conditional)
    {
        AllMembers = allMembers;
        Conditional = conditional;
    }
    public PreserveAttribute()
    {
    }
}

[Preserve(AllMembers = true)]
public class BaseDto
{
    [JsonProperty("Id")]
    public Guid Id { get; set; }
}


public abstract class BaseModel : ReactiveObject
{
    public DateTime CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime ModifiedDate { get; set; }
    public Guid ModifiedBy { get; set; }

    public DateTime? DeletedDate { get; set; }
    public Guid? DeletedBy { get; set; }

    public bool IsDeleted { get; set; } = false;

    public bool IsView { get; set; }
}

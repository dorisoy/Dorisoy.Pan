namespace Dorisoy.Pan.Models;

public class DocumentTokenDto : BaseDto
{
    public Guid DocumentId { get; set; }
    public Guid Token { get; set; }
    public DateTime CreatedDate { get; set; }
}

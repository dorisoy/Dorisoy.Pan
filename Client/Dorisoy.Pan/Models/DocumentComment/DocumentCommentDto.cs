namespace Dorisoy.Pan.Models;

public class DocumentCommentDto : BaseDto
{
    public DateTime CreatedDate { get; set; }
    public string Comment { get; set; }
    public string UserName { get; set; }
}

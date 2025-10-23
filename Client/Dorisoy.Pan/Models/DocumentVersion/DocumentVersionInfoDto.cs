namespace Dorisoy.Pan.Models;

public class DocumentVersionInfoDto : BaseDto
{
    public long Size { get; set; }
    public string UserName { get; set; }
    public DateTime ModifiedDate { get; set; }
    public bool IsCurrentVersion { get; set; }
}

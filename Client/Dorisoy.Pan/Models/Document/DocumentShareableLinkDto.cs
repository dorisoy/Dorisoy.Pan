namespace Dorisoy.PanClient.Models;

public class DocumentShareableLinkDto : BaseDto
{
    public Guid DocumentId { get; set; }
    public string DocumentName { get; set; }
    public string Extension { get; set; }
    public DateTime? LinkExpiryTime { get; set; }
    public string Password { get; set; }
    public string LinkCode { get; set; }
    public bool IsLinkExpired { get; set; }
    public bool IsAllowDownload { get; set; }
    public bool HasPassword { get; set; }
}

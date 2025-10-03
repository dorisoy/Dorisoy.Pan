namespace Dorisoy.PanClient.Models;

public class UserNotificationDto : BaseDto
{
    public Guid? DocumentId { get; set; }
    public Guid? FolderId { get; set; }
    public string FolderName { get; set; }
    public string DocumentName { get; set; }
    public string DocumentThumbnail { get; set; }
    public DateTime CreatedDate { get; set; }
    public string FromUserName { get; set; }
    public Guid FromUserId { get; set; }
    public string Extension { get; set; }
    public bool IsRead { get; set; }
}

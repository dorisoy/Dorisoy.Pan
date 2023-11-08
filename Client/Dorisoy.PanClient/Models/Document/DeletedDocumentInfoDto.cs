namespace Dorisoy.PanClient.Models;

public class DeletedDocumentInfoDto : BaseDto
{
    public string Name { get; set; }
    public string ThumbnailPath { get; set; }
    public DateTime? DeletedDate { get; set; }
}

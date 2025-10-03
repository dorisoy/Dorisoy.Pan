namespace Dorisoy.PanClient.Models;

public class DownloadDocumentDto : BaseDto
{
    public string Name { get; set; }
    public string FolderPath { get; set; }
    public string OriginalFolderPath { get; set; }
    public string Path { get; set; }
    public Guid FolderId { get; set; }
}

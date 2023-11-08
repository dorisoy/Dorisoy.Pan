namespace Dorisoy.PanClient.Commands;

public class DownloadDocumentCommand
{
    public Guid Id { get; set; }
    public bool IsVersion { get; set; }
    public bool IsFromPreview { get; set; } = false;
    public string Token { get; set; }
}

namespace Dorisoy.PanClient.Commands;

public class UploadDocumentCommand
{
    public Guid FolderId { get; set; }
    //public IFormFileCollection Documents { get; set; }
    public string FullPath { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? UserId { get; set; }
}

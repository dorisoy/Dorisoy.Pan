namespace Dorisoy.Pan.Commands;

public class GetDocumentPathByTokenCommand
{
    public Guid Id { get; set; }
    public Guid Token { get; set; }
    public bool IsVersion { get; set; }
}

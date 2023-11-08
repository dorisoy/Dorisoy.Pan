namespace Dorisoy.PanClient.Models
{
    public class VirtualFolderUserModel : ReactiveObject
    {
        [Reactive] public Guid FolderId { get; set; }
        [Reactive] public Guid UserId { get; set; }
        [Reactive] public VirtualFolderModel VirtualFolder { get; set; }
        [Reactive] public UserModel User { get; set; }
    }
}

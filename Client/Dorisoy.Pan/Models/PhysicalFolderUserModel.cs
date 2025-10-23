namespace Dorisoy.Pan.Models
{
    public class PhysicalFolderUserModel
    {
        public Guid FolderId { get; set; }
        public Guid UserId { get; set; }
        public PhysicalFolderModel PhysicalFolder { get; set; }
        public UserModel User { get; set; }
    }
}

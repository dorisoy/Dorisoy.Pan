namespace Dorisoy.PanClient.Models
{
    public class PhysicalFolderModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public PhysicalFolderModel Parent { get; set; }
        public string Size { get; set; }
        public ICollection<PhysicalFolderModel> Children { get; set; } = new List<PhysicalFolderModel>();
        public ICollection<PhysicalFolderUserModel> PhysicalFolderUsers { get; set; } = new List<PhysicalFolderUserModel>();
    }
}

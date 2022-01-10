namespace Dorisoy.Pan.Data.Resources
{
    public class UserResource : ResourceParameter
    {
        public UserResource() : base("Email")
        {
        }

        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IsActive { get; set; }
        public string  FolderId { get; set; }
        public string DocumentId { get; set; }
        public string Type { get; set; }

    }
}

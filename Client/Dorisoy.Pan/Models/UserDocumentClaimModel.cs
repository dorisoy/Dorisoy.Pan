namespace Dorisoy.Pan.Models
{
    public class UserDocumentClaimModel : BaseModel
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public Guid UserId { get; set; }
    }
}

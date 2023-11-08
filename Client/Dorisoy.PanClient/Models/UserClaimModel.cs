namespace Dorisoy.PanClient.Models
{
    /// <summary>
    /// 用户权限
    /// </summary>
    public class UserClaimModel
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        [Reactive] public string ClaimType { get; set; }
        [Reactive] public string ClaimValue { get; set; }
        public Guid ActionId { get; set; }
    }
}

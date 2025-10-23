namespace Dorisoy.Pan.Models
{
    /// <summary>
    /// 角色权限
    /// </summary>
    public class RoleClaimModel
    {
        public int Id { get; set; }
        public Guid RoleId { get; set; }
        [Reactive] public string ClaimType { get; set; }
        [Reactive] public string ClaimValue { get; set; }
        public Guid ActionId { get; set; }
    }


}

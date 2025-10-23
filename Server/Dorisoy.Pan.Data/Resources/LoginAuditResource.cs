namespace Dorisoy.Pan.Data.Resources
{
    public class LoginAuditResource : ResourceParameter
    {
        public LoginAuditResource() : base("LoginTime")
        {
        }

        public string UserName { get; set; }
    }
}

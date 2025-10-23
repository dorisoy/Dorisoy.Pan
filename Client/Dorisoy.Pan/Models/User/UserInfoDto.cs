namespace Dorisoy.Pan.Models;

public class UserInfoDto : BaseDto
{
    public string Email { get; set; }
    public string RaleName { get; set; }
    public bool IsOwner { get; set; }
    public string ProfilePhoto { get; set; }
}

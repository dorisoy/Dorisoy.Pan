namespace Dorisoy.PanClient.Models;

public class UserInfoToken : BaseDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string ConnectionId { get; set; }
    public string IP { get; set; }
}

namespace Dorisoy.PanClient.Models;

public class UserInfoToken : BaseDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string ConnectionId { get; set; }
}

namespace FF.Articles.Backend.Common.Dtos;
public class UserDto
{
    public long Id { get; set; }
    public string UserAccount { get; set; }
    public string UserPassword { get; set; }
    public string? UserEmail { get; set; }
    public string? UserName { get; set; }
    public string? UserAvatar { get; set; }
    public string? UserProfile { get; set; }
    /// <summary>
    /// User role: user, admin, dev 
    /// </summary>
    public string UserRole { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public int IsDelete { get; set; }
}

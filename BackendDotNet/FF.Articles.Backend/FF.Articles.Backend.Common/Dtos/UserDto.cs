using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.Common.Dtos;

/// <summary>
/// Copy from User; Transfer user data between API microservices
/// </summary>
public class UserDto: BaseEntity
{
    public string UserAccount { get; set; }
    public string UserPassword { get; set; }
    public string? UserEmail { get; set; }
    public string? UserName { get; set; }
    public string? UserAvatar { get; set; }
    public string? UserProfile { get; set; }
    /// <summary>
    /// User role: admin, user, editor 
    /// </summary>
    public string UserRole { get; set; }
}

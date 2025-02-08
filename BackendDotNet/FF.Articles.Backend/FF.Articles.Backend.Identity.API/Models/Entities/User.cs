using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.Identity.API.Models.Entities;
public class User : BaseEntity
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


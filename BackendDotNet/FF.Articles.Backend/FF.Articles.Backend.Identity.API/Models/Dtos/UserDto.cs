using System;

namespace FF.Articles.Backend.Identity.API.Models.Dtos;

/// <summary>
/// This reponse is for admins to get user information
/// </summary>
public class UserDto
{
    public long Id { get; set; }
    public string UserAccount { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserAvatar { get; set; }
    public string? UserProfile { get; set; }
    public string UserRole { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }

}

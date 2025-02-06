using System;

namespace FF.Articles.Backend.Identity.API.Models.Responses;

public class UserResponse
{
        public long Id { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }
        
        public string? UserAvatar { get; set; }

        public string? UserProfile { get; set; }

        public string UserRole { get; set; }

        public DateTime CreateTime { get; set; }

}

namespace FF.Articles.Backend.Identity.API.Models.Requests
{
    public class UserUpdateRequest
    {
        public long Id { get; set; }
        public string? UserEmail { get; set; }
        public string UserName { get; set; }
        public string? UserAvatar { get; set; }
        public string UserProfile { get; set; }
        /// <summary>
        /// User role: admin, user, editor 
        /// </summary>
        public string UserRole { get; set; }
    }
}

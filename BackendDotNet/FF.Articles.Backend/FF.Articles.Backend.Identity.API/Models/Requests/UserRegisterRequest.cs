namespace FF.Articles.Backend.Identity.API.Models.Requests;
public class UserRegisterRequest
{
    public String UserAccount { get; set; }
    public String UserPassword { get; set; }
    public String confirmPassword { get; set; }
}
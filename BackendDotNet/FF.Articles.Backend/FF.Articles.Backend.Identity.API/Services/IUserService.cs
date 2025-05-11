namespace FF.Articles.Backend.Identity.API.Services;

public interface IUserService : IBaseService<User>
{
    Task<long> UserRegister(string userAccount, string userPassword, string checkPassword);
    Task<LoginUserDto> UserLogin(string userAccount, string userPassword, HttpRequest request);
    Task<User> GetLoginUser(HttpRequest request);
    Task<User?> GetUserByEmail(string email);
    Task<bool> IsAdmin(HttpRequest request);
    bool IsAdmin(User user);
    Task SignInUser(User user, HttpContext httpContext);
    Task SignOutUser(HttpRequest httpRequest);

}
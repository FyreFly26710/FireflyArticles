namespace FF.Articles.Backend.Identity.API.Repositories;
public interface IUserRepository : IBaseRepository<User, IdentityDbContext>
{
    Task<User?> GetUserByAccount(string account);
    Task<User?> GetUserByEmail(string email);
}

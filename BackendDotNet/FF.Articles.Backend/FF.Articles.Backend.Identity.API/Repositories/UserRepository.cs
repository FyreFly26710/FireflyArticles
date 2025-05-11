namespace FF.Articles.Backend.Identity.API.Repositories;
public class UserRepository : BaseRepository<User, IdentityDbContext>, IUserRepository
{
    public UserRepository(IdentityDbContext _context) : base(_context)
    {
    }
    public async Task<User?> GetUserByAccount(string account)
    {
        if (string.IsNullOrEmpty(account))
            return null;

        return await GetQueryable()
            .FirstOrDefaultAsync(u => account == u.UserAccount);
    }
    public async Task<User?> GetUserByEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return null;

        string emailLower = email.ToLower();
        return await GetQueryable()
            .FirstOrDefaultAsync(u => emailLower == (u.UserEmail ?? "").ToLower());
    }
}

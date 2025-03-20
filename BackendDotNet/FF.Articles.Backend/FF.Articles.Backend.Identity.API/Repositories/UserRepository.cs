using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace FF.Articles.Backend.Identity.API.Repositories;
public class UserRepository : BaseRepository<User, IdentityDbContext>, IUserRepository
{
    public UserRepository(IdentityDbContext _context) : base(_context)
    {
    }
    public async Task<User?> GetUserByAccount(string account)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(u => account.ToLower() == (u.UserAccount ?? "").ToLower());
    }
    public async Task<User?> GetUserByEmail(string email)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(u => email.ToLower() == (u.UserEmail ?? "").ToLower());
    }
}

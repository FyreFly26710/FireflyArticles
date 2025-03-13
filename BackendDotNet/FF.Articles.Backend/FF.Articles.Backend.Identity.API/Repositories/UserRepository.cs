using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.Models.Entities;

namespace FF.Articles.Backend.Identity.API.Repositories;
public class UserRepository : BaseRepository<User, IdentityDbContext>, IUserRepository
{
    public UserRepository(IdentityDbContext _context) : base(_context)
    {
    }
}

using FF.Articles.Backend.Common.Bases.Interfaces;
using FF.Articles.Backend.Identity.API.Infrastructure;
using FF.Articles.Backend.Identity.API.Models.Entities;

namespace FF.Articles.Backend.Identity.API.Repositories;
public interface IUserRepository : IBaseRepository<User, IdentityDbContext>
{
    Task<User?> GetUserByAccount(string account);
    Task<User?> GetUserByEmail(string email);
}

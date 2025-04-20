using System;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Models.Entities;

namespace FF.Articles.Backend.AI.API.Interfaces.Repositories;

public interface ISessionRepository : IBaseRepository<Session, AIDbContext>
{
    Task<List<Session>> GetSessionsByUserId(long userId);
}

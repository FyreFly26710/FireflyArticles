using System;
using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Models.Entities;
using FF.Articles.Backend.AI.API.Interfaces.Repositories;

namespace FF.Articles.Backend.AI.API.Repositories;

public class SystemMessageRepository : BaseRepository<SystemMessage, AIDbContext>, ISystemMessageRepository
{
    public SystemMessageRepository(AIDbContext context) : base(context)
    {
    }
}

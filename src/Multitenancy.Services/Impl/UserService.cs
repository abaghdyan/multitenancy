using Microsoft.EntityFrameworkCore;
using Multitenancy.Data.Master;
using Multitenancy.Data.Master.Entities;
using Multitenancy.Services.Abstractions;

namespace Multitenancy.Services.Impl;

public class UserService : IUserService
{
    private readonly MasterDbContext _masterDbContext;

    public UserService(MasterDbContext masterDbContext)
    {
        _masterDbContext = masterDbContext;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var users = await _masterDbContext.Users.IgnoreQueryFilters().ToListAsync();
        return users;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var users = await _masterDbContext.Users.ToListAsync();
        return users;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var user = await _masterDbContext.Users.FirstOrDefaultAsync(i => i.Id == id);
        return user;
    }
}

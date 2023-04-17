using Multitenancy.Data.Master.Entities;

namespace Multitenancy.Services.Abstractions;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<List<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
}

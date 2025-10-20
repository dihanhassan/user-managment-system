using TodoApp.Domain.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface IUserRepo
    {
        Task<List<User>?> GetAllAsync();
        Task<User?> UpdateAsync(User user);
        Task<int> RemoveAsync(int id);
        Task<User> AddAsync(User user);
        Task<User?> GetByIdAsync(int id);
    }
}

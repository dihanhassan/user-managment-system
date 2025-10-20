using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Repo;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Static;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Repo
{
    public class UserRepo(
        ICacheService cacheService,
        EFDbContext context
        ) : IUserRepo 
    {
        private readonly ICacheService _cacheService = cacheService;
        private readonly EFDbContext _context = context;
        public async Task<List<User>?> GetAllAsync()
        {
            try
            {
                var  cacheKey = CacheKeyPattern.All;
                var cachedUsers = await _cacheService.GetAsync<List<User>>(cacheKey);

                if (cachedUsers != null) 
                    return cachedUsers;

                var users = await _context.Users
                            .Where(u => !u.IsDeleted)
                            .ToListAsync();

                if (users != null)   
                    await _cacheService.SetAsync(cacheKey, users, TimeSpan.FromHours(1));

                return users;
            }
            catch (Exception) 
            {
                throw;
            }
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                var cacheKey = CacheKeyPattern.ById(id.ToString());
                var cachedUsers = await _cacheService.GetAsync<User>(cacheKey);

                if (cachedUsers != null)
                    return cachedUsers;

                var user = await _context.Users.FindAsync(id);

                if (user == null)
                    await _cacheService.SetAsync(cacheKey, user, TimeSpan.FromHours(1));

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<User> AddAsync(User user)
        {
            try
            {
                var rowEffect = await _context.AddAsync<User>(user);
                await _context.SaveChangesAsync();
                return user;
               
            }
            catch (Exception) 
            {
                throw;
            }
        }
        public async Task<int> RemoveAsync(int id)
        {
            try
            {
                var cacheKey = CacheKeyPattern.ById(id.ToString());
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return 0;

                user.IsDeleted = true;
                user.UpdatedOn = DateTime.UtcNow;

                _context.Users.Update(user);
                var result = await _context.SaveChangesAsync();
                await _cacheService.RemoveAsync(cacheKey);
                return result; 
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<User?> UpdateAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

                if (existingUser == null)
                    return null; 

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.Mobile = user.Mobile;
                existingUser.Address = user.Address;
                existingUser.DateOfBirth = user.DateOfBirth;
                existingUser.UpdatedOn = DateTime.UtcNow;

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                var cacheKey = CacheKeyPattern.ById(user.Id.ToString());
                await _cacheService.SetAsync(cacheKey, existingUser, TimeSpan.FromHours(1));

                return existingUser;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

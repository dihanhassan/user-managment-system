using UserManagement.Application.Interfaces;
using UserManagement.Application.Services;
using UserManagement.Domain.Interfaces;
using UserManagement.Repo;
using UserManagment.Cache.Redis;

namespace UserManagement.DependencyExtensions
{
    public static class RegisterService
    {
        public static void AddServices(this IServiceCollection services)
        {
            #region A
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            #endregion A
        }
    }
}

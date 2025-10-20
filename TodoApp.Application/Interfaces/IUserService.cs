using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Application.Dtos;
using UserManagement.Application.Dtos.Common;

namespace UserManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task<Response<List<UserDto>>> GetAllAsync();
        Task<Response<UserDto>> UpdateAsync(UserDto user);
        Task<Response<int>> RemoveAsync(int id);
        Task<Response<UserDto>> AddAsync(UserDto user);
        Task<Response<UserDto>> GetByIdAsync(int id);
    }
}

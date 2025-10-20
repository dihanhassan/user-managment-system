using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Application.Dtos;
using TodoApp.Domain.Entities;
using UserManagement.Application.Dtos.Common;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Services
{
    public class UserService(
        IUserRepo repo,
        IMapper mapper
        ) : IUserService
    {
        private readonly IUserRepo _repo = repo;
        private readonly IMapper _mapper = mapper;
        public async Task<Response<UserDto>> AddAsync(UserDto userDto)
        {
            try
            {
                var finalResponse = new Response<UserDto>();
                var data = await _repo.AddAsync(_mapper.Map<User>(userDto));
                if (data != null)
                {
                    finalResponse.Data = _mapper.Map<UserDto>(data);
                    finalResponse.Status = "200";
                    finalResponse.Message = "Success!";

                }
                else
                {
                    finalResponse.Status = "901";
                    finalResponse.Message = "Failed!";
                }
                return finalResponse;
            }
            catch (Exception) 
            {
                throw;
            }
        }

        public async Task<Response<List<UserDto>>> GetAllAsync()
        {
            try
            {
                var finalResponse = new Response<List<UserDto>>();
                var data = await _repo.GetAllAsync();
                if (data != null)
                {
                    finalResponse.Data = _mapper.Map<List<UserDto>>(data);
                    finalResponse.Status = "200";
                    finalResponse.Message = "Success!";

                }
                else
                {
                    finalResponse.Status = "901";
                    finalResponse.Message = "Failed!";
                }
                return finalResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Response<UserDto>> GetByIdAsync(int id)
        {
            try
            {
                var finalResponse = new Response<UserDto>();
                var data = await _repo.GetByIdAsync(id);
                if (data != null)
                {
                    finalResponse.Data = _mapper.Map<UserDto>(data);
                    finalResponse.Status = "200";
                    finalResponse.Message = "Success!";

                }
                else
                {
                    finalResponse.Status = "901";
                    finalResponse.Message = "Failed!";
                }
                return finalResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Response<int>> RemoveAsync(int id)
        {
            try
            {
                var finalResponse = new Response<int>();
                var data = await _repo.RemoveAsync(id);
                if (data > 0)
                {
                    finalResponse.Status = "200";
                    finalResponse.Message = "Success!";

                }
                else
                {
                    finalResponse.Status = "901";
                    finalResponse.Message = "Failed!";
                }
                return finalResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Response<UserDto>> UpdateAsync(UserDto userDto)
        {
            try
            {
                var finalResponse = new Response<UserDto>();
                var data = await _repo.UpdateAsync(_mapper.Map<User>(userDto));
                if (data != null)
                {
                    finalResponse.Data = _mapper.Map<UserDto>(data);
                    finalResponse.Status = "200";
                    finalResponse.Message = "Success!";

                }
                else
                {
                    finalResponse.Status = "901";
                    finalResponse.Message = "Failed!";
                }
                return finalResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

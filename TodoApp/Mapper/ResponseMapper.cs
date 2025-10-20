using AutoMapper;
using TodoApp.Application.Dtos;
using TodoApp.Domain.Entities;

namespace UserManagement.Mapper
{
    public class ResponseMapper : Profile
    {
        public ResponseMapper()
        {
            CreateMap<User, UserDto>();
        }
    }
}

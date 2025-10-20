using AutoMapper;
using TodoApp.Application.Dtos;
using TodoApp.Domain.Entities;

namespace UserManagement.Mapper
{
    public class RequestMapper : Profile
    {
        public RequestMapper()
        {
            CreateMap<UserDto, User>();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Dtos;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Services;

namespace UserManagement.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController(IUserService service) : ControllerBase
    {
        private readonly IUserService _service = service;

        [HttpPost("get")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _service.GetAllAsync());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(await _service.GetByIdAsync(id));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] UserDto userDto)
        {
            try
            {
                return Ok(await _service.AddAsync(userDto));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UserDto userDto)
        {
            try
            {
                return Ok(await _service.UpdateAsync(userDto));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] int id)
        {
            try
            {
                return Ok(await _service.RemoveAsync(id));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}

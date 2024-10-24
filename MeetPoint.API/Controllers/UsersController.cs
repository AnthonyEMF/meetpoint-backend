using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Users;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeetPoint.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            this._usersService = usersService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<List<UserDto>>>> GetAll(string searchTerm = "", int page = 1)
        {
            var response = await _usersService.GetAllUsersAsync(searchTerm, page);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<UserDto>>> Get(Guid id)
        {
            var response = await _usersService.GetUserByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<UserDto>>> Create(UserCreateDto dto)
        {
            var response = await _usersService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<UserDto>>> Edit(UserEditDto dto, Guid id)
        {
            var response = await _usersService.EditAsync(dto, id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<UserDto>>> Delete(Guid id)
        {
            var response = await _usersService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}

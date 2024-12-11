using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Users;

namespace MeetPoint.API.Services.Interfaces
{
    public interface IUsersService
	{
		Task<ResponseDto<PaginationDto<List<UserDto>>>> GetAllUsersAsync(string searchTerm = "", int page = 1);
		Task<ResponseDto<UserDto>> GetUserByIdAsync(string id);
		Task<ResponseDto<UserDto>> CreateAsync(UserCreateDto dto);
		Task<ResponseDto<UserDto>> EditAsync(UserEditDto dto, string id);
		Task<ResponseDto<UserDto>> DeleteAsync(string id);
		Task<ResponseDto<UserDto>> ToggleBlockUserAsync(string id);
	}
}

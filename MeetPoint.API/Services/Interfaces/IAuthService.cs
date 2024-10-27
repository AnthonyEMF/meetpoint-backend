using MeetPoint.API.Dtos.Auth;
using MeetPoint.API.Dtos.Common;

namespace MeetPoint.API.Services.Interfaces
{
	public interface IAuthService
	{
		Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto);
		Task<ResponseDto<LoginResponseDto>> RegisterAsync(RegisterDto dto);
	}
}

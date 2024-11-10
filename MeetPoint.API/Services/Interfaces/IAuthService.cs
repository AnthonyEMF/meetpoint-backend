using MeetPoint.API.Dtos.Auth;
using MeetPoint.API.Dtos.Common;
using System.Security.Claims;

namespace MeetPoint.API.Services.Interfaces
{
	public interface IAuthService
	{
		Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto);
		Task<ResponseDto<LoginResponseDto>> RegisterAsync(RegisterDto dto);
		Task<ResponseDto<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
		ClaimsPrincipal GetTokenPrincipal(string token);
	}
}

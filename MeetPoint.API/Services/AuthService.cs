using MeetPoint.API.Constants;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Auth;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MeetPoint.API.Services
{
	public class AuthService : IAuthService
	{
		private readonly SignInManager<UserEntity> _signInManager;
		private readonly UserManager<UserEntity> _userManager;
		private readonly IConfiguration _configuration;

		public AuthService(
			SignInManager<UserEntity> signInManager,
			UserManager<UserEntity> userManager,
			IConfiguration configuration)
        {
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._configuration = configuration;
		}

		public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto)
		{
			var result = await _signInManager.
				PasswordSignInAsync(dto.Email, dto.Password, isPersistent: false, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				// Generación del token
				var userEntity = await _userManager.FindByEmailAsync(dto.Email);

				// ClaimList
				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Email, userEntity.Email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim("UserId", userEntity.Id),
				};

				var userRoles = await _userManager.GetRolesAsync(userEntity);
				foreach (var role in userRoles)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, role));
				}

				var jwtToken = GetToken(authClaims);

				return new ResponseDto<LoginResponseDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.LOGIN_SUCCESS,
					Data = new LoginResponseDto
					{
						Email = userEntity.Email,
						Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
						TokenExpiration = jwtToken.ValidTo,
					}
				};
			}

			return new ResponseDto<LoginResponseDto>
			{
				Status = false,
				StatusCode = 401,
				Message = MessagesConstant.LOGIN_ERROR
			};
		}

		public async Task<ResponseDto<LoginResponseDto>> RegisterAsync(RegisterDto dto)
		{
			var user = new UserEntity
			{
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				Location = dto.Location,
				Email = dto.Email,
				UserName = dto.Email,
			};

			// Crear nuevo usuario
			var result = await _userManager.CreateAsync(user, dto.Password);

			if (result.Succeeded)
			{
				var userEntity = await _userManager.FindByEmailAsync(dto.Email);

				// Asignar rol al nuevo usuario
				await _userManager.AddToRoleAsync(userEntity, RolesConstant.USER);

				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Email, userEntity.Email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim("UserId", userEntity.Id),
					new Claim(ClaimTypes.Role, RolesConstant.USER)
				};

				var jwtToken = GetToken(authClaims);

				return new ResponseDto<LoginResponseDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.REGISTER_SUCCESS,
					Data = new LoginResponseDto
					{
						Email = userEntity.Email,
						Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
						TokenExpiration = jwtToken.ValidTo
					}
				};
			}

			return new ResponseDto<LoginResponseDto>
			{
				StatusCode = 400,
				Status = false,
				Message = MessagesConstant.REGISTER_ERROR
			};
		}

		private JwtSecurityToken GetToken(List<Claim> authClaims)
		{
			var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8
				.GetBytes(_configuration["JWT:Secret"]));

			return new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddHours(1),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
			);
		}
	}
}

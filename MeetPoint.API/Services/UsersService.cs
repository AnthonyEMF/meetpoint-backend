﻿using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Users;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
    public class UsersService : IUsersService
    {
        private readonly MeetPointContext _context;
		private readonly UserManager<UserEntity> _userManager;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;
		private readonly int PAGE_SIZE;

		public UsersService(
            MeetPointContext context,
			UserManager<UserEntity> userManager,
            IMapper mapper,
            ILogger<UsersService> logger,
            IConfiguration configuration)
        {
            this._context = context;
			this._userManager = userManager;
			this._mapper = mapper;
			this._logger = logger;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

		public async Task<ResponseDto<PaginationDto<List<UserDto>>>> GetAllUsersAsync(string searchTerm = "", int page = 1)
		{
			int startIndex = (page - 1) * PAGE_SIZE;
			var usersEntityQuery = _userManager.Users.AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				usersEntityQuery = usersEntityQuery
					.Where(u => (u.FirstName + " " + u.LastName + " " + u.Email)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			int totalUsers = await usersEntityQuery.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalUsers / PAGE_SIZE);

			var usersEntity = await usersEntityQuery
				.OrderBy(e => e.FirstName)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var usersDto = new List<UserDto>();
			foreach (var user in usersEntity)
			{
				// Obtener rol del usuario
				var userRole = await _userManager.GetRolesAsync(user);

				usersDto.Add(new UserDto
				{
					Id = user.Id,
					Roles = userRole.ToList(),
					Email = user.Email,
					PasswordHash = user.PasswordHash,
					FirstName = user.FirstName,
					LastName = user.LastName,
				});
			}

			return new ResponseDto<PaginationDto<List<UserDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<UserDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalUsers,
					TotalPages = totalPages,
					Items = usersDto,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages,
				}
			};
		}

		public async Task<ResponseDto<UserDto>> GetUserByIdAsync(string id)
		{
			var userEntity = await _userManager.Users
				.Include(u => u.OrganizedEvents).ThenInclude(e => e.Category)
				.Include(u => u.Attendances).ThenInclude(a => a.Event)
				.Include(u => u.Comments).ThenInclude(c => c.Event)
				.FirstOrDefaultAsync(u => u.Id == id);

			if (userEntity is null)
			{
				return new ResponseDto<UserDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

			var userRole = await _userManager.GetRolesAsync(userEntity);
			var userDto = _mapper.Map<UserDto>(userEntity);
			userDto.Roles = userRole.ToList();

			return new ResponseDto<UserDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORD_FOUND,
				Data = userDto
			};
		}

		public async Task<ResponseDto<UserDto>> CreateAsync(UserCreateDto dto)
		{
			// Validar si el email ya existe
			var existingEmail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
			if (existingEmail is not null)
			{
				return new ResponseDto<UserDto>
				{
					StatusCode = 400,
					Status = false,
					Message = "El email ingresado ya está registrado."
				};
			}

			var user = _mapper.Map<UserEntity>(dto);

			// Crear nuevo usuario y pasar contraseña
			var result = await _userManager.CreateAsync(user, dto.Password);

			if (result.Succeeded)
			{
				// Buscar el nuevo usuario
				var userEntity = await _userManager.FindByEmailAsync(dto.Email);

				// Asignar el rol
				await _userManager.AddToRoleAsync(userEntity, dto.Role);
				await _context.SaveChangesAsync();

				// Obtener rol del usuario
				var userRole = await _userManager.GetRolesAsync(user);

				return new ResponseDto<UserDto>
				{
					StatusCode = 200,
					Status = true,
					Message = "Usuario creado correctamente",
					Data = new UserDto
					{
						Id = user.Id,
						Roles = userRole.ToList(),
						Email = user.Email,
						PasswordHash = user.PasswordHash,
						FirstName = user.FirstName,
						LastName = user.LastName,
						Location = user.Location,
					}
				};
			}

			return new ResponseDto<UserDto>
			{
				StatusCode = 400,
				Status = false,
				Message = MessagesConstant.CREATE_ERROR
			};
		}

		public async Task<ResponseDto<UserDto>> EditAsync(UserEditDto dto, string id)
		{
			try
			{
				var user = await _userManager.Users
					.Include(u => u.Attendances).ThenInclude(a => a.Event)
					.Include(u => u.Comments).ThenInclude(c => c.Event)
					.FirstOrDefaultAsync(u => u.Id == id);

				if (user is null)
				{
					return new ResponseDto<UserDto>
					{
						StatusCode = 404,
						Status = false,
						Message = MessagesConstant.RECORD_NOT_FOUND
					};
				}

				// Validacion si se intenta actualizar el email
				if (user.Email != dto.Email)
				{
					var existingEmail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
					if (existingEmail is not null)
					{
						return new ResponseDto<UserDto>
						{
							StatusCode = 400,
							Status = false,
							Message = "El email ingresado ya está registrado."
						};
					}
				}

				// Actualizar los datos del usuario
				user.FirstName = dto.FirstName;
				user.LastName = dto.LastName;
				user.Email = dto.Email;
				user.UserName = dto.Email;
				user.Location = dto.Location;

				// Actualizar la contraseña
				if (!string.IsNullOrEmpty(dto.Password))
				{
					var passwordResetResult = await _userManager.RemovePasswordAsync(user);
					if (!passwordResetResult.Succeeded)
					{
						return new ResponseDto<UserDto>
						{
							StatusCode = 400,
							Status = false,
							Message = "No se pudo actualizar la contraseña."
						};
					}

					var passwordResult = await _userManager.AddPasswordAsync(user, dto.Password);
					if (!passwordResult.Succeeded)
					{
						return new ResponseDto<UserDto>
						{
							StatusCode = 400,
							Status = false,
							Message = "No se pudo agregar la nueva contraseña."
						};
					}
				}

				// Actualizar el rol
				if (!string.IsNullOrEmpty(dto.Role))
				{
					var existingRoles = await _userManager.GetRolesAsync(user);
					await _userManager.RemoveFromRolesAsync(user, existingRoles);
					await _userManager.AddToRoleAsync(user, dto.Role);
				}

				var result = await _userManager.UpdateAsync(user);
				await _context.SaveChangesAsync();

				if (!result.Succeeded)
				{
					return new ResponseDto<UserDto>
					{
						StatusCode = 400,
						Status = false,
						Message = MessagesConstant.UPDATE_ERROR
					};
				}

				var userRole = await _userManager.GetRolesAsync(user);
				var userDto = _mapper.Map<UserDto>(user);
				userDto.Roles = userRole.ToList();

				return new ResponseDto<UserDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.UPDATE_SUCCESS,
					Data = userDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.UPDATE_ERROR);
				return new ResponseDto<UserDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.UPDATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<UserDto>> DeleteAsync(string id)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var userEntity = await _userManager.Users
						.Include(u => u.Attendances)
						.Include(u => u.Comments)
						.FirstOrDefaultAsync(u => u.Id == id);

					if (userEntity is null)
					{
						return new ResponseDto<UserDto>
						{
							StatusCode = 404,
							Status = false,
							Message = MessagesConstant.RECORD_NOT_FOUND
						};
					}

					// Eliminar comentarios del usuario
					_context.Comments.RemoveRange(userEntity.Comments);

					// Eliminar asistencias del usuario
					_context.Attendances.RemoveRange(userEntity.Attendances);

					// Remover los roles del usuario
					var currentRoles = await _userManager.GetRolesAsync(userEntity);
					if (currentRoles.Any())
					{
						await _userManager.RemoveFromRolesAsync(userEntity, currentRoles);
					}

					// Eliminar
					var result = await _userManager.DeleteAsync(userEntity);

					if (result.Succeeded)
					{
						// Guardar
						await _context.SaveChangesAsync();
						await transaction.CommitAsync();

						return new ResponseDto<UserDto>
						{
							StatusCode = 200,
							Status = true,
							Message = MessagesConstant.DELETE_SUCCESS
						};
					}

					return new ResponseDto<UserDto>
					{
						StatusCode = 400,
						Status = false,
						Message = MessagesConstant.DELETE_ERROR
					};
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					_logger.LogError(ex, MessagesConstant.DELETE_ERROR);
					return new ResponseDto<UserDto>
					{
						StatusCode = 500,
						Status = false,
						Message = MessagesConstant.DELETE_ERROR
					};
				}
			}
		}
	}
}

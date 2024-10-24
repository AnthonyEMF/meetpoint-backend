using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Users;
using MeetPoint.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
    public class UsersService : IUsersService
    {
        private readonly MeetPointContext _context;
        private readonly IMapper _mapper;
		private readonly ILogger _logger;
		private readonly int PAGE_SIZE;

		public UsersService(
            MeetPointContext context,
            IMapper mapper,
            ILogger<UsersService> logger,
            IConfiguration configuration)
        {
            this._context = context;
            this._mapper = mapper;
			this._logger = logger;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

        public async Task<ResponseDto<PaginationDto<List<UserDto>>>> GetAllUsersAsync(string searchTerm = "", int page = 1)
        {
			int startIndex = (page - 1) * PAGE_SIZE;

			var usersEntityQuery = _context.Users
				.Include(u => u.OrganizedEvents).ThenInclude(e => e.Category)
				.Include(u => u.Attendances).ThenInclude(a => a.Event)
				.Include(u => u.Comments).ThenInclude(c => c.Event)
				.Where(u => u.CreatedDate <= DateTime.Now);

			if (!string.IsNullOrEmpty(searchTerm))
			{
				usersEntityQuery = usersEntityQuery
					.Where(u => (u.FirstName + " " + u.LastName + " " + u.Email)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			int totalUsers = await usersEntityQuery.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalUsers / PAGE_SIZE);

			var usersEntity = await usersEntityQuery
				.OrderByDescending(e => e.CreatedDate)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var usersDto = _mapper.Map<List<UserDto>>(usersEntity);

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

        public async Task<ResponseDto<UserDto>> GetUserByIdAsync(Guid id)
        {
            var userEntity = await _context.Users
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

			var userDto = _mapper.Map<UserDto>(userEntity);

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
			try
			{
				// Validar si el email ya existe
				var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
				if (existingEmail is not null)
				{
					return new ResponseDto<UserDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "El email ingresado ya está registrado."
					};
				}

				var userEntity = _mapper.Map<UserEntity>(dto);

				_context.Users.Add(userEntity);
				await _context.SaveChangesAsync();

				var userDto = _mapper.Map<UserDto>(userEntity);

				return new ResponseDto<UserDto>
				{
					StatusCode = 201,
					Status = true,
					Message = MessagesConstant.CREATE_SUCCESS,
					Data = userDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
				return new ResponseDto<UserDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.CREATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<UserDto>> EditAsync(UserEditDto dto, Guid id)
		{
			try
			{
				var userEntity = await _context.Users
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

				// Validacion si se intenta actualizar el email
				if (userEntity.Email != dto.Email)
				{
					var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
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

				_mapper.Map(dto, userEntity);

				_context.Users.Update(userEntity);
				await _context.SaveChangesAsync();

				var userDto = _mapper.Map<UserDto>(userEntity);

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

		public async Task<ResponseDto<UserDto>> DeleteAsync(Guid id)
        {
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var userEntity = await _context.Users
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

					// Eliminar el usuario
					_context.Users.Remove(userEntity);

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return new ResponseDto<UserDto>
					{
						StatusCode = 200,
						Status = true,
						Message = MessagesConstant.DELETE_SUCCESS
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

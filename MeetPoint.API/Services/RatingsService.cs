using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Ratings;
using MeetPoint.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
	public class RatingsService : IRatingsService
	{
		private readonly MeetPointContext _context;
		private readonly IAuditService _auditService;
		private readonly IMapper _mapper;
		private readonly ILogger<RatingsService> _logger;
		private readonly int PAGE_SIZE;

		public RatingsService(
			MeetPointContext context,
			IAuditService auditService,
			IMapper mapper,
			ILogger<RatingsService> logger,
			IConfiguration configuration)
        {
			this._context = context;
			this._auditService = auditService;
			this._mapper = mapper;
			this._logger = logger;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

		public async Task<ResponseDto<PaginationDto<List<RatingDto>>>> GetPaginationRatingsAsync(string searchTerm = "", int page = 1)
		{
			int startIndex = (page - 1) * PAGE_SIZE;
			var ratingsEntityQuery = _context.Ratings
				.Include(r => r.Rater)
				.Include(r => r.Event)
				.Include(r => r.Organizer)
				.Where(r => r.CreatedDate <= DateTime.Now);

			if (!string.IsNullOrEmpty(searchTerm))
			{
				ratingsEntityQuery = ratingsEntityQuery
					.Where(r => (r.Rater + " " + r.Event + " " + r.Organizer + " " + r.RatingDate)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			int totalRatings = await ratingsEntityQuery.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalRatings / PAGE_SIZE);

			var ratingsEntity = await ratingsEntityQuery
				.OrderByDescending(r => r.RatingDate)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var ratingsDto = _mapper.Map<List<RatingDto>>(ratingsEntity);

			return new ResponseDto<PaginationDto<List<RatingDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<RatingDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalRatings,
					TotalPages = totalPages,
					Items = ratingsDto,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages,
				}
			};
		}

		public async Task<ResponseDto<RatingDto>> GetRatingByIdAsync(Guid id)
		{
			var ratingEntity = await _context.Ratings
				.Include(r => r.Rater)
				.Include(r => r.Event)
				.Include(r => r.Organizer)
				.FirstOrDefaultAsync(r => r.Id == id);

			if (ratingEntity is null)
			{
				return new ResponseDto<RatingDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

			var ratingDto = _mapper.Map<RatingDto>(ratingEntity);

			return new ResponseDto<RatingDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORD_FOUND,
				Data = ratingDto
			};
		}

		public async Task<ResponseDto<RatingDto>> CreateAsync(RatingCreateDto dto)
		{
			try
			{
				// El RaterId corresponde al usuario en sesión
				dto.RaterId = _auditService.GetUserId();

				// Validar que el OrganizerId existe
				var existingOrganizer = await _context.Users.FindAsync(dto.OrganizerId);
				if (existingOrganizer is null)
				{
					return new ResponseDto<RatingDto>
					{
						StatusCode = 404,
						Status = false,
						Message = $"OrganizerId: {MessagesConstant.RECORD_NOT_FOUND}"
					};
				}

				// Validar que el EventId existe
				var existingEvent = await _context.Events.FindAsync(dto.EventId);
				if (existingEvent is null)
				{
					return new ResponseDto<RatingDto>
					{
						StatusCode = 404,
						Status = false,
						Message = $"EventId: {MessagesConstant.RECORD_NOT_FOUND}"
					};
				}

				// Validar que la fecha del evento ya expiró
				if (existingEvent.Date >= DateTime.Now)
				{
					return new ResponseDto<RatingDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "El evento aún no ha terminado."
					};
				}

				// Validar que el OrganizerId corresponde al EventId
				if (existingEvent.OrganizerId != dto.OrganizerId)
				{
					return new ResponseDto<RatingDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "El OrganizerId no es el creador del evento."
					};
				}

				// Validar que los organizadores no se puedan dar rating a si mismos
				if (dto.OrganizerId == dto.RaterId)
				{
					return new ResponseDto<RatingDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "El RaterId y OrganizerId no pueden ser el mismo."
					};
				}

				// Validar que el RaterId tiene una asistencia registrada al EventId
				var raterAttendance = await _context.Attendances
					.AnyAsync(a => a.UserId == dto.RaterId && a.EventId == dto.EventId);
				if (!raterAttendance)
                {
					return new ResponseDto<RatingDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "El RaterId no se encuentra en la lista de asistencia del evento."
					};
				}

				// Verificar si el usuario ya tiene un rating registrado para el organizador y el evento
				var existingReport = await _context.Ratings
					.AnyAsync(r => r.OrganizerId == dto.OrganizerId && r.RaterId == dto.RaterId && r.EventId == dto.EventId);
				if (existingReport)
				{
					return new ResponseDto<RatingDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "El usuario ya tiene un rating registrado para el organizador y evento."
					};
				}

				var ratingEntity = _mapper.Map<RatingEntity>(dto);
				ratingEntity.RatingDate = DateTime.Now;

				_context.Ratings.Add(ratingEntity);
				await _context.SaveChangesAsync();

				var ratingDto = _mapper.Map<RatingDto>(ratingEntity);

				return new ResponseDto<RatingDto>
				{
					StatusCode = 201,
					Status = true,
					Message = MessagesConstant.CREATE_SUCCESS,
					Data = ratingDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
				return new ResponseDto<RatingDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.CREATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<RatingDto>> EditAsync(RatingEditDto dto, Guid id)
		{
			try
			{
				var ratingEntity = await _context.Ratings
					.Include(r => r.Rater)
					.Include(r => r.Event)
					.Include(r => r.Organizer)
					.FirstOrDefaultAsync(r => r.Id == id);

				if (ratingEntity is null)
				{
					return new ResponseDto<RatingDto>
					{
						StatusCode = 404,
						Status = false,
						Message = MessagesConstant.RECORD_NOT_FOUND
					};
				}

				_mapper.Map(dto, ratingEntity);

				_context.Ratings.Update(ratingEntity);
				await _context.SaveChangesAsync();

				var ratingDto = _mapper.Map<RatingDto>(ratingEntity);

				return new ResponseDto<RatingDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.UPDATE_SUCCESS,
					Data = ratingDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
				return new ResponseDto<RatingDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.CREATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<RatingDto>> DeleteAsync(Guid id)
		{
			try
			{
				var ratingEntity = await _context.Ratings
					.Include(r => r.Rater)
					.Include(r => r.Event)
					.Include(r => r.Organizer)
					.FirstOrDefaultAsync(r => r.Id == id);

				if (ratingEntity is null)
				{
					return new ResponseDto<RatingDto>
					{
						StatusCode = 404,
						Status = false,
						Message = MessagesConstant.RECORD_NOT_FOUND
					};
				}

				_context.Ratings.Remove(ratingEntity);
				await _context.SaveChangesAsync();

				var ratingDto = _mapper.Map<RatingDto>(ratingEntity);

				return new ResponseDto<RatingDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.DELETE_SUCCESS,
					Data = ratingDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
				return new ResponseDto<RatingDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.CREATE_ERROR
				};
			}
		}
	}
}

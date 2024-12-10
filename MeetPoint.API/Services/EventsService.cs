using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Events;
using MeetPoint.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
	public class EventsService : IEventsService
	{
		private readonly MeetPointContext _context;
		private readonly IAuditService _auditService;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;
		private readonly int PAGE_SIZE;

		public EventsService(
			MeetPointContext context,
			IAuditService auditService,
			IMapper mapper,
			ILogger<EventsService> logger,
			IConfiguration configuration)
        {
			this._context = context;
			this._auditService = auditService;
			this._mapper = mapper;
			this._logger = logger;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

        public async Task<ResponseDto<PaginationDto<List<EventDto>>>> GetAllEventsAsync(string searchTerm = "", int page = 1)
		{
			int startIndex = (page - 1) * PAGE_SIZE;

			var eventsEntityQuery = _context.Events
				.Include(e => e.Category)
				.Include(e => e.Organizer)
				.Include(e => e.Attendances).ThenInclude(a => a.User)
				.Include(e => e.Comments).ThenInclude(c => c.User)
				.Where(e => e.PublicationDate <= DateTime.Now);

			if (!string.IsNullOrEmpty(searchTerm))
			{
				eventsEntityQuery = eventsEntityQuery
					.Where(e => (e.Title + " " + e.Category.Name + " " + e.Description)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			int totalEvents = await eventsEntityQuery.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalEvents / PAGE_SIZE);

			var eventsEntity = await eventsEntityQuery
				.OrderByDescending(e => e.Date)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var eventsDto = _mapper.Map<List<EventDto>>(eventsEntity);

			return new ResponseDto<PaginationDto<List<EventDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<EventDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalEvents,
					TotalPages = totalPages,
					Items = eventsDto,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages,
				}
			};
		}

		public async Task<ResponseDto<EventDto>> GetEventByIdAsync(Guid id)
		{
			var eventEntity = await _context.Events
				.Include(e => e.Category)
				.Include(e => e.Organizer)
				.Include(e => e.Attendances).ThenInclude(a => a.User)
				.Include(e => e.Comments).ThenInclude(c => c.User)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (eventEntity is null)
			{
				return new ResponseDto<EventDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

			var eventDto = _mapper.Map<EventDto>(eventEntity);

			return new ResponseDto<EventDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORD_FOUND,
				Data = eventDto
			};
		}

		public async Task<ResponseDto<EventDto>> CreateAsync(EventCreateDto dto)
		{
			try
			{
				// Validar que la categoría del evento existe
				var existingCategory = await _context.Categories.FindAsync(dto.CategoryId);
				if (existingCategory is null)
				{
					return new ResponseDto<EventDto>
					{
						StatusCode = 404,
						Status = false,
						Message = $"CategoryId: {MessagesConstant.RECORD_NOT_FOUND}"
					};
				}

                // Validar que la fecha del evento no ha pasado aún
                if (dto.Date <= DateTime.Now)
                {
					return new ResponseDto<EventDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "La fecha ingresada ya pasó."
					};
				}

                var eventEntity = _mapper.Map<EventEntity>(dto);
				eventEntity.OrganizerId = _auditService.GetUserId();

				eventEntity.PublicationDate = DateTime.Now;

				_context.Events.Add(eventEntity);
				await _context.SaveChangesAsync();

				var eventDto = _mapper.Map<EventDto>(eventEntity);

				return new ResponseDto<EventDto>
				{
					StatusCode = 201,
					Status = true,
					Message = MessagesConstant.CREATE_SUCCESS,
					Data = eventDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
				return new ResponseDto<EventDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.CREATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<EventDto>> EditAsync(EventEditDto dto, Guid id)
		{
			try
			{
				var eventEntity = await _context.Events
					.Include(e => e.Category)
					.Include(e => e.Organizer)
					.FirstOrDefaultAsync(e => e.Id == id);

				if (eventEntity is null)
				{
					return new ResponseDto<EventDto>
					{
						StatusCode = 404,
						Status = false,
						Message = MessagesConstant.RECORD_NOT_FOUND
					};
				}

				// Validar que la categoría existe si se va a editar
				if (dto.CategoryId != eventEntity.CategoryId)
				{
					var category = await _context.Categories.FindAsync(dto.CategoryId);
					if (category is null)
					{
						return new ResponseDto<EventDto>
						{
							StatusCode = 404,
							Status = false,
							Message = $"CategoryId: {MessagesConstant.RECORD_NOT_FOUND}"
						};
					}
					eventEntity.CategoryId = dto.CategoryId;
				}

				// Validar que la fecha del evento no ha pasado aún
				if (dto.Date <= DateTime.Now)
				{
					return new ResponseDto<EventDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "La fecha ingresada ya pasó."
					};
				}

				// Actualizar propiedades del evento
				_mapper.Map(dto, eventEntity);

				_context.Events.Update(eventEntity);
				await _context.SaveChangesAsync();

				var eventDto = _mapper.Map<EventDto>(eventEntity);

				return new ResponseDto<EventDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.UPDATE_SUCCESS,
					Data = eventDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.UPDATE_ERROR);
				return new ResponseDto<EventDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.UPDATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<EventDto>> DeleteAsync(Guid id)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var eventEntity = await _context.Events
						.Include(e => e.Attendances)
						.Include(e => e.Comments)
						.FirstOrDefaultAsync(e => e.Id == id);

					if (eventEntity is null)
					{
						return new ResponseDto<EventDto>
						{
							StatusCode = 404,
							Status = false,
							Message = MessagesConstant.RECORD_NOT_FOUND
						};
					}

					// Eliminar los comentarios del evento
					_context.Comments.RemoveRange(eventEntity.Comments);

					// Eliminar las asistencias del evento
					_context.Attendances.RemoveRange(eventEntity.Attendances);

					// Eliminar el evento
					_context.Events.Remove(eventEntity);

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return new ResponseDto<EventDto>
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
					return new ResponseDto<EventDto>
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

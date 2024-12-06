using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Attendances;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Events;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
    public class AttendancesService : IAttendancesService
    {
        private readonly MeetPointContext _context;
		private readonly UserManager<UserEntity> _userManager;
		private readonly IAuditService _auditService;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;
        private readonly int PAGE_SIZE;

		public AttendancesService(
            MeetPointContext context,
			UserManager<UserEntity> userManager,
			IAuditService auditService,
            IMapper mapper,
            ILogger<AttendancesService> logger,
            IConfiguration configuration)
        {
            this._context = context;
			this._userManager = userManager;
			this._auditService = auditService;
			this._mapper = mapper;
			this._logger = logger;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

        public async Task<ResponseDto<PaginationDto<List<AttendanceDto>>>> GetAllAttendancesAsync(string searchTerm = "", int page = 1)
        {
			int startIndex = (page - 1) * PAGE_SIZE;

            var attendancesEntityQuery = _context.Attendances
                .Include(a => a.User)
                .Include(a => a.Event)
				.Where(a => a.CreatedDate <= DateTime.Now);

			if (!string.IsNullOrEmpty(searchTerm))
			{
				attendancesEntityQuery = attendancesEntityQuery
					.Where(a => (a.State + " " + a.User.FirstName + " " + a.User.LastName)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			var totalAttendances = await attendancesEntityQuery.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalAttendances / PAGE_SIZE);

			var attendancesEntity = await attendancesEntityQuery
				.OrderBy(u => u.CreatedDate)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

            var attendancesDto = _mapper.Map<List<AttendanceDto>>(attendancesEntity);

			return new ResponseDto<PaginationDto<List<AttendanceDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<AttendanceDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalAttendances,
					TotalPages = totalPages,
					Items = attendancesDto,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages
				}
			};
		}

        public async Task<ResponseDto<AttendanceDto>> GetAttendanceByIdAsync(Guid id)
        {
            var attendanceEntity = await _context.Attendances
                .Include(a => a.User)
                .Include(a => a.Event)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendanceEntity is null)
            {
				return new ResponseDto<AttendanceDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

            var attendanceDto = _mapper.Map<AttendanceDto>(attendanceEntity);

			return new ResponseDto<AttendanceDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORD_FOUND,
				Data = attendanceDto
			};
		}

        public async Task<ResponseDto<AttendanceDto>> CreateAsync(AttendanceCreateDto dto)
        {
			try
			{
				// El UserId corresponde al usuario en sesión
				dto.UserId = _auditService.GetUserId();

				// Validar que el evento de la asistencia existe
				var existingEvent = await _context.Events.FindAsync(dto.EventId);
				if (existingEvent is null)
				{
					return new ResponseDto<AttendanceDto>
					{
						StatusCode = 404,
						Status = false,
						Message = $"EventId: {MessagesConstant.RECORD_NOT_FOUND}"
					};
				}

				// Validar que la fecha del evento no ha expirado
				if (existingEvent.Date < DateTime.Now)
				{
					return new ResponseDto<AttendanceDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "El evento ya expiró."
					};
				}

				// Verificar si el usuario ya tiene una asistencia para el evento
				var existingAttendance = await _context.Attendances
					.AnyAsync(a => a.EventId == dto.EventId && a.UserId == dto.UserId);

				if (existingAttendance)
				{
					return new ResponseDto<AttendanceDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "El usuario ya tiene una asistencia registrada para el evento."
					};
				}

				var attendanceEntity = _mapper.Map<AttendanceEntity>(dto);

				_context.Attendances.Add(attendanceEntity);
				await _context.SaveChangesAsync();

				var attendanceDto = _mapper.Map<AttendanceDto>(attendanceEntity);

				return new ResponseDto<AttendanceDto>
				{
					StatusCode = 201,
					Status = true,
					Message = MessagesConstant.CREATE_SUCCESS,
					Data = attendanceDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
				return new ResponseDto<AttendanceDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.CREATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<AttendanceDto>> EditAsync(AttendanceEditDto dto, Guid id)
		{
			try
			{
				var attendanceEntity = await _context.Attendances
					.Include(a => a.User)
					.Include(a => a.Event)
					.FirstOrDefaultAsync(a => a.Id == id);

				if (attendanceEntity is null)
				{
					return new ResponseDto<AttendanceDto>
					{
						StatusCode = 404,
						Status = false,
						Message = MessagesConstant.RECORD_NOT_FOUND
					};
				}

				_mapper.Map(dto, attendanceEntity);

				_context.Attendances.Update(attendanceEntity);
				await _context.SaveChangesAsync();

				var attendanceDto = _mapper.Map<AttendanceDto>(attendanceEntity);

				return new ResponseDto<AttendanceDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.UPDATE_SUCCESS,
					Data = attendanceDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.UPDATE_ERROR);
				return new ResponseDto<AttendanceDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.UPDATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<AttendanceDto>> DeleteAsync(Guid id)
        {
			try
			{
				var attendanceEntity = await _context.Attendances
					.Include(a => a.User)
					.Include(a => a.Event)
					.FirstOrDefaultAsync(a => a.Id == id);

				if (attendanceEntity is null)
				{
					return new ResponseDto<AttendanceDto>
					{
						StatusCode = 404,
						Status = false,
						Message = MessagesConstant.RECORD_NOT_FOUND
					};
				}

				_context.Attendances.Remove(attendanceEntity);
				await _context.SaveChangesAsync();

				var attendanceDto = _mapper.Map<AttendanceDto>(attendanceEntity);

				return new ResponseDto<AttendanceDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.DELETE_SUCCESS,
					Data = attendanceDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.DELETE_ERROR);
				return new ResponseDto<AttendanceDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.DELETE_ERROR
				};
			}
		}
    }
}

using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Reports;
using MeetPoint.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
	public class ReportsService : IReportsService
	{
		private readonly MeetPointContext _context;
		private readonly IAuditService _auditService;
		private readonly IMapper _mapper;
		private readonly ILogger<ReportsService> _logger;
		private readonly int PAGE_SIZE;

		public ReportsService(
			MeetPointContext context,
			IAuditService auditService,
			IMapper mapper,
			ILogger<ReportsService> logger,
			IConfiguration configuration)
        {
			this._context = context;
			this._auditService = auditService;
			this._mapper = mapper;
			this._logger = logger;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

		public async Task<ResponseDto<PaginationDto<List<ReportDto>>>> GetPaginationReportsAsync(string searchTerm = "", int page = 1)
		{
			int startIndex = (page - 1) * PAGE_SIZE;
			var reportsEntityQuery = _context.Reports
				.Include(r => r.Reporter)
				.Include(r => r.Organizer)
				.Where(r => r.CreatedDate <= DateTime.Now);

			if (!string.IsNullOrEmpty(searchTerm))
			{
				reportsEntityQuery = reportsEntityQuery
					.Where(r => (r.Reporter.FirstName + " " + r.Reason + " " + r.Organizer.FirstName + " " + r.ReportDate)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			int totalReports = await reportsEntityQuery.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalReports / PAGE_SIZE);

			var reportsEntity = await reportsEntityQuery
				.OrderByDescending(r => r.ReportDate)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var reportsDto = new List<ReportDto>();
			foreach (var report in reportsEntity)
			{
				reportsDto.Add(new ReportDto
				{
					Id = report.Id,
					ReporterId = report.ReporterId,
					ReporterName = $"{report.Reporter.FirstName} {report.Reporter.LastName}",
					OrganizerId = report.OrganizerId,
					OrganizerName = $"{report.Organizer.FirstName} {report.Organizer.LastName}",
					Reason = report.Reason,
					ReportDate = report.ReportDate,
				});
			}

			return new ResponseDto<PaginationDto<List<ReportDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<ReportDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalReports,
					TotalPages = totalPages,
					Items = reportsDto,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages,
				}
			};
		}

		public async Task<ResponseDto<ReportDto>> GetReportByIdAsync(Guid id)
		{
			var reportEntity = await _context.Reports
				.Include(r => r.Reporter)
				.Include(r => r.Organizer)
				.FirstOrDefaultAsync(r => r.Id == id);

			if (reportEntity is null)
			{
				return new ResponseDto<ReportDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

			var reportDto = _mapper.Map<ReportDto>(reportEntity);

			return new ResponseDto<ReportDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORD_FOUND,
				Data = reportDto
			};
		}

		public async Task<ResponseDto<ReportDto>> CreateAsync(ReportCreateDto dto)
		{
			try
			{
				// El ReporterId corresponde al usuario en sesión
				dto.ReporterId = _auditService.GetUserId();

				// Validar que el usuario que esta siendo reportado existe
				var existingOrganizer = await _context.Users.FindAsync(dto.OrganizerId);
				if (existingOrganizer is null)
				{
					return new ResponseDto<ReportDto>
					{
						StatusCode = 404,
						Status = false,
						Message = $"OrganizerId: {MessagesConstant.RECORD_NOT_FOUND}"
					};
				}

				// Validar que los usuarios no se puedan reportar a si mismos
				if (dto.OrganizerId == dto.ReporterId)
				{
					return new ResponseDto<ReportDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "Los usuarios no pueden reportarse a si mismos"
					};
				}

				// Verificar si el usuario ya tiene un reporte registrado para el organizador
				var existingReport = await _context.Reports
					.AnyAsync(r => r.OrganizerId == dto.OrganizerId && r.ReporterId == dto.ReporterId);

				if (existingReport)
				{
					return new ResponseDto<ReportDto>
					{
						StatusCode = 400,
						Status = false,
						Message = "El usuario ya tiene un reporte registrado para el organizador."
					};
				}

				var reportEntity = _mapper.Map<ReportEntity>(dto);
				reportEntity.ReportDate = DateTime.Now;

				_context.Reports.Add(reportEntity);
				await _context.SaveChangesAsync();

				var reportDto = _mapper.Map<ReportDto>(reportEntity);

				return new ResponseDto<ReportDto>
				{
					StatusCode = 201,
					Status = true,
					Message = MessagesConstant.CREATE_SUCCESS,
					Data = reportDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
				return new ResponseDto<ReportDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.CREATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<ReportDto>> EditAsync(ReportEditDto dto, Guid id)
		{
			try
			{
				var reportEntity = await _context.Reports
					.Include(r => r.Reporter)
					.Include(r => r.Organizer)
					.FirstOrDefaultAsync(r => r.Id == id);

				if (reportEntity is null)
				{
					return new ResponseDto<ReportDto>
					{
						StatusCode = 404,
						Status = false,
						Message = MessagesConstant.RECORD_NOT_FOUND
					};
				}

				_mapper.Map(dto, reportEntity);

				_context.Reports.Update(reportEntity);
				await _context.SaveChangesAsync();

				var reportDto = _mapper.Map<ReportDto>(reportEntity);

				return new ResponseDto<ReportDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.UPDATE_SUCCESS,
					Data = reportDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
				return new ResponseDto<ReportDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.CREATE_ERROR
				};
			}
		}

		public async Task<ResponseDto<ReportDto>> DeleteAsync(Guid id)
		{
			try
			{
				var reportEntity = await _context.Reports
					.Include(r => r.Reporter)
					.Include(r => r.Organizer)
					.FirstOrDefaultAsync(r => r.Id == id);

				if (reportEntity is null)
				{
					return new ResponseDto<ReportDto>
					{
						StatusCode = 404,
						Status = false,
						Message = MessagesConstant.RECORD_NOT_FOUND
					};
				}

				_context.Reports.Remove(reportEntity);
				await _context.SaveChangesAsync();

				var reportDto = _mapper.Map<ReportDto>(reportEntity);

				return new ResponseDto<ReportDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.DELETE_SUCCESS,
					Data = reportDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
				return new ResponseDto<ReportDto>
				{
					StatusCode = 500,
					Status = false,
					Message = MessagesConstant.CREATE_ERROR
				};
			}
		}
	}
}

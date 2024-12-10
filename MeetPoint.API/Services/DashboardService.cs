using AutoMapper;
using MeetPoint.API.Constants;
using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Dtos.Common;
using MeetPoint.API.Dtos.Dashboard;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Services
{
	public class DashboardService : IDashboardService
	{
		private readonly MeetPointContext _context;
		private readonly UserManager<UserEntity> _userManager;
		private readonly IMapper _mapper;

		public DashboardService(MeetPointContext context, UserManager<UserEntity> userManager, IMapper mapper)
        {
			this._context = context;
			this._userManager = userManager;
			this._mapper = mapper;
		}

		public async Task<ResponseDto<DashboardDto>> GetDashboardAsync()
		{
			var eventsQuery = _context.Events
				.Include(e => e.Attendances).ThenInclude(a => a.User)
				.Include(e => e.Comments).ThenInclude(c => c.User)
				.Where(e => e.PublicationDate <= DateTime.Now);
			var events = await eventsQuery.OrderByDescending(e => e.CreatedDate).Take(5).ToListAsync();

			var users = await _context.Users.OrderBy(u => u.FirstName).Take(5).ToListAsync();
			var categories = await _context.Categories.OrderByDescending(c => c.CreatedDate).Take(5).ToListAsync();
			var reports = await _context.Reports.OrderByDescending(r => r.CreatedDate).ToListAsync();

			var usersDto = _mapper.Map<List<DashboardUserDto>>(users);
			foreach (var userDto in usersDto)
			{
				var userEntity = users.First(u => u.Id == userDto.Id);
				userDto.Roles = (await _userManager.GetRolesAsync(userEntity)).ToList();
			}

			var dashboardDto = new DashboardDto
			{
				UsersCount = await _context.Users.CountAsync(),
				EventsCount = await _context.Events.CountAsync(),
				AttendancesCount = await _context.Attendances.CountAsync(),
				CommentsCount = await _context.Comments.CountAsync(),
				Categories = _mapper.Map<List<DashboardCategoryDto>>(categories),
				Users = usersDto,
				Events = _mapper.Map<List<DashboardEventDto>>(events),
				Reports = _mapper.Map<List<DashboardReportDto>>(reports)
			};

			return new ResponseDto<DashboardDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = dashboardDto
			};
		}
	}
}

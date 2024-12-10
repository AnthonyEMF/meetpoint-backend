namespace MeetPoint.API.Dtos.Dashboard
{
	public class DashboardDto
	{
		public int UsersCount { get; set; }
		public int EventsCount { get; set; }
		public int AttendancesCount { get; set; }
		public int CommentsCount { get; set; }

		public List<DashboardUserDto> Users { get; set; }
		public List<DashboardEventDto> Events { get; set; }
		public List<DashboardCategoryDto> Categories { get; set; }
		public List<DashboardReportDto> Reports { get; set; }
	}
}

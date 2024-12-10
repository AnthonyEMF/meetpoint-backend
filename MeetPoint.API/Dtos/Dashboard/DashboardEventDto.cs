using MeetPoint.API.Dtos.Attendances;
using MeetPoint.API.Dtos.Comments;

namespace MeetPoint.API.Dtos.Dashboard
{
	public class DashboardEventDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int AttendancesCount { get; set; }
		public int CommentsCount { get; set; }
		public DateTime PublicationDate { get; set; }
	}
}

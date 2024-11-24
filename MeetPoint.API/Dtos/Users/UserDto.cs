using MeetPoint.API.Dtos.Attendances;
using MeetPoint.API.Dtos.Comments;
using MeetPoint.API.Dtos.Events;

namespace MeetPoint.API.Dtos.Users
{
	public class UserDto
	{
		public string Id { get; set; }
        public List<string> Roles { get; set; }
		public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
		public string LastName { get; set; }
        public string Location { get; set; }
        public int EventsCount { get; set; }
        public int AttendancesCount { get; set; }

        public List<EventDto> OrganizedEvents { get; set; }
		public List<AttendanceDto> Attendances { get; set; }
		public List<CommentDto> Comments { get; set; }
	}
}

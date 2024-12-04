using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MeetPoint.API.Dtos.Categories;
using MeetPoint.API.Dtos.Attendances;
using MeetPoint.API.Dtos.Comments;

namespace MeetPoint.API.Dtos.Events
{
	public class EventDto
	{
        public Guid Id { get; set; }
		public Guid CategoryId { get; set; }
		public string CategoryName { get; set; }
		public string OrganizerId { get; set; }
		public string OrganizerName { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Ubication { get; set; }
		public DateTime Date { get; set; }
        public DateTime PublicationDate { get; set; }
		public int AttendancesCount { get; set; }
		public int CommentsCount { get; set; }

		public List<AttendanceDto> Attendances { get; set; }
		public List<CommentDto> Comments { get; set; }
	}
}

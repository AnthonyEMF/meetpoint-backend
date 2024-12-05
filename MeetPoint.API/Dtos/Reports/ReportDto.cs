using MeetPoint.API.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Reports
{
	public class ReportDto
	{
        public Guid Id { get; set; }
        public string ReporterId { get; set; }
		public string ReporterName { get; set; }
		public string OrganizerId { get; set; }
		public string OrganizerName { get; set; }
		public string Reason { get; set; }
		public DateTime ReportDate { get; set; }
	}
}

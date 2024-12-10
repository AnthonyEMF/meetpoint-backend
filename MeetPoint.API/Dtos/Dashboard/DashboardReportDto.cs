namespace MeetPoint.API.Dtos.Dashboard
{
	public class DashboardReportDto
	{
		public Guid Id { get; set; }
		public string ReporterName { get; set; }
		public string OrganizerName { get; set; }
		public string Reason { get; set; }
		public DateTime ReportDate { get; set; }
	}
}

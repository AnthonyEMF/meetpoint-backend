namespace MeetPoint.API.Dtos.Memberships
{
	public class MembershipDto
	{
        public Guid Id { get; set; }
		public string UserId { get; set; }
        public string UserName { get; set; }
		public string Type { get; set; }
		public decimal Price { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}

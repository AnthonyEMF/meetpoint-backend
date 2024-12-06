using MeetPoint.API.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Ratings
{
	public class RatingDto
	{
        public Guid Id { get; set; }
		public string RaterId { get; set; }
        public string RaterName { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; }
        public string OrganizerId { get; set; }
        public string OrganizerName { get; set; }
        public decimal Score { get; set; }
		public DateTime RatingDate { get; set; }
	}
}

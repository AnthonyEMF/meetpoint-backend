﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Comments
{
	public class CommentDto
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string UserName { get; set; }
		public Guid EventId { get; set; }
		public string EventTitle { get; set; }
		public string Content { get; set; }
		public DateTime PublicationDate { get; set; }
	}
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetPoint.API.Database.Entities
{
	[Table("ratings", Schema = "dbo")]
	public class RatingEntity : BaseEntity
	{
		[Column("rater_id")]
		public string RaterId { get; set; }
		[ForeignKey(nameof(RaterId))]
		public virtual UserEntity Rater { get; set; }

		[Column("event_id")]
		public Guid EventId { get; set; }
		[ForeignKey(nameof(EventId))]
		public virtual EventEntity Event { get; set; }

		[Column("organizer_id")]
		public string OrganizerId { get; set; }
		[ForeignKey(nameof(OrganizerId))]
		public virtual UserEntity Organizer { get; set; }

		[Range(0, 5, ErrorMessage = "La puntuación del rating debe estar entre 0 y 5.")]
		[Precision(2, 1)]
		[Required(ErrorMessage = "La puntuación del rating es requerida.")]
		[Column("score")]
		public decimal Score { get; set; }

		[Column("rating_date")]
        public DateTime RatingDate { get; set; }

		// Propiedades para el RatingConfiguration
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}

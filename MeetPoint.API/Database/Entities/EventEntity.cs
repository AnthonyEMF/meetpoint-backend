using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetPoint.API.Database.Entities
{
	[Table("events", Schema = "dbo")]
	public class EventEntity : BaseEntity
	{
		[Column("category_id")]
        public Guid CategoryId { get; set; }
		[ForeignKey(nameof(CategoryId))]
		public virtual CategoryEntity Category { get; set; }

		[Column("organizer_id")]
		public string OrganizerId { get; set; }
		[ForeignKey(nameof(OrganizerId))]
		public virtual UserEntity Organizer { get; set; }

		[Required(ErrorMessage = "El campo título es requerido.")]
		[StringLength(50)]
		[Column("title")]
		public string Title { get; set; }

		[Required(ErrorMessage = "El campo descripción es requerido.")]
		[StringLength(300)]
		[Column("description")]
		public string Description { get; set; }

		[Required(ErrorMessage = "El campo ubicación es requerido.")]
		[StringLength(100)]
		[Column("ubication")]
		public string Ubication { get; set; }

		[Required(ErrorMessage = "El campo fecha es requerido.")]
		[Column("date")]
		public DateTime Date { get; set; }

		[Column("publication_date")]
        public DateTime PublicationDate { get; set; }

		// Propiedad calculada: Cantidad de asistencias del evento.
		[NotMapped]
		public int AttendancesCount => Attendances?.Count ?? 0;

		// Propiedad calculada: Cantidad de comentarios del evento.
		[NotMapped]
		public int CommentsCount => Comments?.Count ?? 0;

		// Navegación: Asistencias del Evento.
		public virtual ICollection<AttendanceEntity> Attendances { get; set; }

		// Navegación: Comentarios del Evento.
		public virtual ICollection<CommentEntity> Comments { get; set; }

		// Propiedades para el EventConfiguration
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}

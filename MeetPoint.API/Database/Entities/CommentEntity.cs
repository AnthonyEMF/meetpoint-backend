using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetPoint.API.Database.Entities
{
	[Table("comments", Schema = "dbo")]
	public class CommentEntity : BaseEntity
	{
		[Column("user_id")]
		public string UserId { get; set; }
		[ForeignKey(nameof(UserId))]
		public virtual UserEntity User { get; set; }

		[Column("event_id")]
		public Guid EventId { get; set; }
		[ForeignKey(nameof(EventId))]
		public virtual EventEntity Event { get; set; }

		[Required(ErrorMessage = "Es requerido escribir el contenido del comentario.")]
		[StringLength(200)]
		[Column("content")]
		public string Content { get; set; }

		[Column("publication_date")]
		public DateTime PublicationDate { get; set; }

		[Column("parent_id")]
		public Guid? ParentId { get; set; }
		[ForeignKey(nameof(ParentId))]
		public virtual CommentEntity ParentComment { get; set; }

		// Navegación: Respuestas del Comentario.
		public virtual ICollection<CommentEntity> Replies { get; set; }

		// Propiedades para el CommentConfiguration
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}

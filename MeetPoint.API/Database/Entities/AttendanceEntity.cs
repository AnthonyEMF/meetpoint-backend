using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetPoint.API.Database.Entities
{
	[Table("attendances", Schema = "dbo")]
	public class AttendanceEntity : BaseEntity
	{
		[Column("user_id")]
        public string UserId { get; set; }
		[ForeignKey(nameof(UserId))]
		public virtual UserEntity User { get; set; }

		[Column("event_id")]
		public Guid EventId { get; set; }
		[ForeignKey(nameof(EventId))]
		public virtual EventEntity Event { get; set; }

		[Required(ErrorMessage = "Especificar el estado de la asistencia es requerido.")]
		[RegularExpression("^(CONFIRMADO|PENDIENTE|CANCELADO)$", ErrorMessage = "El estado de la asistencia debe ser CONFIRMADO, PENDIENTE o CANCELADO.")]
		[StringLength(10)]
		[Column("state")]
		public string State { get; set; }

		// Propiedades para el AttendanceConfiguration
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Database.Entities
{
	[Table("users", Schema = "dbo")]
	public class UserEntity : BaseEntity
	{
		[Required(ErrorMessage = "El primer nombre es requerido.")]
		[StringLength(50)]
		[Column("first_name")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "El segundo nombre es requerido.")]
		[StringLength(50)]
		[Column("last_name")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "El email es requerido.")]
		[EmailAddress(ErrorMessage = "El formato del email es incorrecto.")]
		[StringLength(200)]
		[Column("email")]
		public string Email { get; set; }

		[MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
		[StringLength(100)]
		[Column("password")]
		public string Password { get; set; }

		[Required(ErrorMessage = "La locación es requerida.")]
		[StringLength(100)]
		[Column("location")]
		public string Location { get; set; }

		// Propiedad calculada: Cantidad de eventos organizados por el usuario.
		[NotMapped]
		public int EventsCount => OrganizedEvents?.Count ?? 0;

		// Propiedad calculada: Cantidad de asistencias del usuario.
		[NotMapped]
		public int AttendancesCount => Attendances?.Count ?? 0;

		// Navegación: Eventos organizados por el usuario.
		public virtual ICollection<EventEntity> OrganizedEvents { get; set; }

		// Navegación: Asistencias del usuario.
		public virtual ICollection<AttendanceEntity> Attendances { get; set; }

		// Navegación: Comentarios del usuario.
		public virtual ICollection<CommentEntity> Comments { get; set; }
	}
}

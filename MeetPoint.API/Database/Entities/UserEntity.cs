using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MeetPoint.API.Database.Entities
{
	public class UserEntity : IdentityUser
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

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

		// Propiedades para el UserConfiguration (?)
		//public virtual UserEntity CreatedByUser { get; set; }
		//public virtual UserEntity UpdatedByUser { get; set; }
	}
}

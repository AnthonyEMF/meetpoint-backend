using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace MeetPoint.API.Database.Entities
{
	public class UserEntity : IdentityUser
	{
		[StringLength(70, MinimumLength = 3)]
		[Required]
		public string FirstName { get; set; }

		[StringLength(70, MinimumLength = 3)]
		[Required]
		public string LastName { get; set; }

		[StringLength(100, MinimumLength = 10)]
		[Required]
		public string Location { get; set; }

		[StringLength(450)]
		public string RefreshToken { get; set; }

		public DateTime RefreshTokenExpire { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool IsBlocked { get; set; } = false;

		// Membresía del usuario
		public virtual MembershipEntity Membership { get; set; }

		// Propiedad calculada: Cantidad de eventos organizados por el usuario.
		[NotMapped]
		public int EventsCount => OrganizedEvents?.Count ?? 0;

		// Propiedad calculada: Cantidad de asistencias del usuario.
		[NotMapped]
		public int AttendancesCount => Attendances?.Count ?? 0;

		// Propiedad calculada: Cantidad de reportes que se le han hecho al usuario.
		[NotMapped]
		public int ReportsCount => Reports?.Count ?? 0;

		// Propiedad calculada: Cantidad de ratings que se le han hecho al usuario.
		[NotMapped]
		public int RatingsCount => Ratings?.Count ?? 0;

		// Propiedad calculada: Promedio de los ratings que se le han hecho al usuario.
		[NotMapped]
		public decimal AverageRating => Ratings?.Count > 0 ? Ratings.Average(r => r.Score) : 0;

		// Navegación: Eventos organizados por el usuario.
		public virtual ICollection<EventEntity> OrganizedEvents { get; set; }

		// Navegación: Asistencias del usuario.
		public virtual ICollection<AttendanceEntity> Attendances { get; set; }

		// Navegación: Comentarios del usuario.
		public virtual ICollection<CommentEntity> Comments { get; set; }

		// Navegación: Reportes que el usuario ha hecho
		public virtual ICollection<ReportEntity> MadeReports { get; set; }

		// Navegación: Reportes hechos al usuario
		public virtual ICollection<ReportEntity> Reports { get; set; }

		// Navegación: Ratings que el usuario ha hecho
		public virtual ICollection<RatingEntity> MadeRatings { get; set; }

		// Navegación: Ratings hechos al usuario
		public virtual ICollection<RatingEntity> Ratings { get; set; }
	}
}

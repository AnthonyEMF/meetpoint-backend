using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetPoint.API.Database.Entities
{
	[Table("reports", Schema = "dbo")]
	public class ReportEntity : BaseEntity
	{
		[Column("reporter_id")]
		public string ReporterId { get; set; }
		[ForeignKey(nameof(ReporterId))]
		public virtual UserEntity Reporter { get; set; }

		[Column("organizer_id")]
		public string OrganizerId { get; set; }
		[ForeignKey(nameof(OrganizerId))]
		public virtual UserEntity Organizer { get; set; }

		[Required(ErrorMessage = "Es requerido escribir la razón del reporte.")]
		[MinLength(10, ErrorMessage = "La razón del reporte debe tener al menos 10 caracteres.")]
		[StringLength(200)]
		[Column("reason")]
		public string Reason { get; set; }

		[Column("report_date")]
		public DateTime ReportDate { get; set; }

		// Propiedades para el ReportConfiguration
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetPoint.API.Database.Entities
{
	[Table("memberships", Schema = "dbo")]
	public class MembershipEntity : BaseEntity
	{
		[Column("user_id")]
		public string UserId { get; set; }
		[ForeignKey(nameof(UserId))]
		public virtual UserEntity User { get; set; }

		[Required(ErrorMessage = "Especificar el tipo de membresía es requerido.")]
		[Column("type")]
		public string Type { get; set; } // "MENSUAL" o "ANUAL"

		[Required(ErrorMessage = "Especificar el precio de la membresía es requerido.")]
		[Column("price")]
		[Precision(18, 2)]
        public decimal Price { get; set; }

		[Required(ErrorMessage = "Especificar la fecha de inicio es requerido.")]
		[Column("start_date")]
		public DateTime StartDate { get; set; }

		[Required(ErrorMessage = "Especificar la fecha de finalización es requerido.")]
		[Column("end_date")]
		public DateTime EndDate { get; set; }

		// Propiedades para el MembershipConfiguration
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}

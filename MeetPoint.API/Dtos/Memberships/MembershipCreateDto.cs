using MeetPoint.API.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Memberships
{
	public class MembershipCreateDto
	{
		public string UserId { get; set; }

		[Required(ErrorMessage = "Especificar el tipo de membresía es requerido.")]
		[RegularExpression("^(Mensual|Anual)$", ErrorMessage = "El tipo de membresía debe ser Mensual o Anual.")]
		public string Type { get; set; }

		[Required(ErrorMessage = "Especificar el precio de la membresía es requerido.")]
		public decimal Price { get; set; }

		[Required(ErrorMessage = "Especificar la fecha de inicio es requerido.")]
		public DateTime StartDate { get; set; }

		[Required(ErrorMessage = "Especificar la fecha de finalización es requerido.")]
		public DateTime EndDate { get; set; }
	}
}

using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Attendances
{
	public class AttendanceEditDto
	{
		[Required(ErrorMessage = "Especificar el estado de la asistencia es requerido.")]
		[RegularExpression("^(CONFIRMADO|PENDIENTE|CANCELADO)$", ErrorMessage = "El estado de la asistencia debe ser CONFIRMADO, PENDIENTE o CANCELADO.")]
		public string State { get; set; }
	}
}

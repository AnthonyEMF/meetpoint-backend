using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Reports
{
	public class ReportCreateDto
	{
		public string ReporterId { get; set; }

		[Required(ErrorMessage = "Es requerido ingresar el Id del organizador.")]
		public string OrganizerId { get; set; }

		[Required(ErrorMessage = "Es requerido escribir la razón del reporte.")]
		[MinLength(10, ErrorMessage = "La razón del reporte debe tener al menos 10 caracteres.")]
		public string Reason { get; set; }
	}
}

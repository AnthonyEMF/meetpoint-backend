using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Ratings
{
	public class RatingCreateDto
	{
		public string RaterId { get; set; }

		[Required(ErrorMessage = "Es requerido ingresar el Id del evento.")]
		public Guid EventId { get; set; }

		[Required(ErrorMessage = "Es requerido ingresar el Id del organizador.")]
		public string OrganizerId { get; set; }

		[Required(ErrorMessage = "La puntuación del rating es requerida.")]
		[Range(0, 5, ErrorMessage = "La puntuación del rating debe estar entre 0 y 5.")]
		public decimal Score { get; set; }
	}
}

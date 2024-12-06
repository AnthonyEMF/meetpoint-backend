using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Ratings
{
	public class RatingEditDto
	{
		[Required(ErrorMessage = "La puntuación del rating es requerida.")]
		[Range(0, 5, ErrorMessage = "La puntuación del rating debe estar entre 0 y 5.")]
		public decimal Score { get; set; }
	}
}

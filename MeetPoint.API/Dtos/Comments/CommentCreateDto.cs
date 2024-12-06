using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Comments
{
	public class CommentCreateDto
	{
		public string UserId { get; set; }
		public Guid EventId { get; set; }

		[Required(ErrorMessage = "Es requerido escribir el contenido del comentario.")]
		public string Content { get; set; }
		public Guid? ParentId { get; set; }
	}
}

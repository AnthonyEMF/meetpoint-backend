using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Categories
{
	public class CategoryCreateDto
	{
		[Required(ErrorMessage = "El campo Nombre es requerido.")]
		public string Name { get; set; }

		[MinLength(10, ErrorMessage = "El campo descripción debe tener al menos 10 caracteres.")]
		public string Description { get; set; }
    }
}

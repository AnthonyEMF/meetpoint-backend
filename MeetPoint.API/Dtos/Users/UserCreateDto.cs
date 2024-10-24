using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Users
{
	public class UserCreateDto
	{
		[Required(ErrorMessage = "El primer nombre es requerido.")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "El segundo nombre es requerido.")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "El email es requerido.")]
		[EmailAddress(ErrorMessage = "El formato del email es incorrecto.")]
		public string Email { get; set; }

		[MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
		public string Password { get; set; }

		[Required(ErrorMessage = "La locación es requerida.")]
		public string Location { get; set; }
	}
}

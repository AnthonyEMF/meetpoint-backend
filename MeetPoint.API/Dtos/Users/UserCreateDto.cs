using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Users
{
	public class UserCreateDto
	{
		[Required(ErrorMessage = "El email del usuario es requerido.")]
		[EmailAddress(ErrorMessage = "El email no es válido.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "La contraseña del usuario es requerida.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "La contraseña debe contener al menos 8 caracteres e incluir minúsculas, mayúsculas, números y caracteres especiales.")]
		public string Password { get; set; }

		[Required(ErrorMessage = "El rol del usuario es requerido.")]
		[RegularExpression("^(ADMIN|ORGANIZER|USER)$", ErrorMessage = "El rol debe ser ADMIN, ORGANIZER o USER.")]
		public string Role { get; set; }

		[Required(ErrorMessage = "El primer nombre es requerido.")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "El segundo nombre es requerido.")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "La locación es requerida.")]
		public string Location { get; set; }
	}
}

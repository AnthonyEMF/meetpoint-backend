using System.ComponentModel.DataAnnotations;

namespace MeetPoint.API.Dtos.Events
{
	public class EventCreateDto
	{
        public Guid CategoryId { get; set; }
        public string OrganizerId { get; set; }

        [Required(ErrorMessage = "El campo título es requerido.")]
		public string Title { get; set; }

		[Required(ErrorMessage = "El campo descripción es requerido.")]
		public string Description { get; set; }

		[Required(ErrorMessage = "El campo ubicación es requerido.")]
		public string Ubication { get; set; }

		[Required(ErrorMessage = "El campo fecha es requerido.")]
		public DateTime Date { get; set; }
	}
}

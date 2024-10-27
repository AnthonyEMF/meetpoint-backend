using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetPoint.API.Database.Entities
{
	[Table("categories", Schema = "dbo")]
	public class CategoryEntity : BaseEntity
	{
        [Required(ErrorMessage = "El campo Nombre es requerido.")]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

		[MinLength(10, ErrorMessage = "El campo descripción debe tener al menos 10 caracteres.")]
		[StringLength(200)]
        [Column("description")]
        public string Description { get; set; }

		// Navegación: Eventos que pertenecen a la Categoría.
		public virtual ICollection<EventEntity> Events { get; set; }

		// Propiedades para el CategoryConfiguration
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}

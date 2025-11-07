using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CongresoTIC.API.Models
{
    [Table("Participantes")]
    public class Participante
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El usuario de Twitter es requerido")]
        [StringLength(50)]
        public string UsuarioTwitter { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ocupación es requerida")]
        [StringLength(100)]
        public string Ocupacion { get; set; } = string.Empty;

        public string? Avatar { get; set; }

        [Required]
        public bool AceptaTerminos { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellidos}";
    }
}
using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Rol
    {
        [Display(Name = "ID Rol")]
        public int id_rol { get; set; }

        [Display(Name = "Nombre Rol")]
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(50)]
        public string? nombre_rol { get; set; }
    }
}



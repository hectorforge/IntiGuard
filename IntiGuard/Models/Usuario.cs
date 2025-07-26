using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntiGuard.Models
{
    public class Usuario
    {
        [Key]
        public int id_usuario { get; set; }

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(100)]
        public string? nombres { get; set; }

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100)]
        public string? apellidos { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [StringLength(100)]
        public string? correo { get; set; }

        [Display(Name = "Clave")]
        [StringLength(255)]
        public string? clave { get; set; }

        //[Display(Name = "ID Rol")]
        [ForeignKey("Rol")]
        public int id_rol { get; set; }

        [Display(Name = "Fecha Registro")]
        public DateTime fecha_registro { get; set; } = DateTime.Now;

    }
}


using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Comprobante
    {
        [Key]
        public int id_comprobante { get; set; }

        [Display(Name = "Tipo Comprobante")]
        [Required(ErrorMessage = "El tipo de comprobante es obligatorio.")]
        [StringLength(50)]
        public string? tipo_comprobante { get; set; }

        [Display(Name = "Número Comprobante")]
        [Required(ErrorMessage = "El número de comprobante es obligatorio.")]
        [StringLength(20)]
        public string? numero_comprobante { get; set; }
    }
}

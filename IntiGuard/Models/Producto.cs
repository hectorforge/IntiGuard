using System.ComponentModel.DataAnnotations.Schema;// For Decimal
using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Producto
    {
        [Display(Name = "Id Producto")]
        public int id_producto { get; set; }

        [Display(Name = "Nombre Producto")]
        public string? nombre_producto { get; set; }

        [Display(Name = "Descripción")]
        public string? descripcion { get; set; }

        public string? marca { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal precio { get; set; }

        [Display(Name = "Stock")]
        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int stock { get; set; }

        [Display(Name = "URL Imagen")]
        [StringLength(500)]
        public string? imagen_url { get; set; }
    }

}

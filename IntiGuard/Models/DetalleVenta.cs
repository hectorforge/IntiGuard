using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class DetalleVenta
    {
        [Key]
        public int id_detalle_venta { get; set; }

        [ForeignKey("Venta")]
        public int id_venta { get; set; }

        [ForeignKey("Producto")]
        public int id_producto { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int cantidad { get; set; }

        [Display(Name = "Precio Unitario")]
        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que cero.")]
        public decimal precio_unitario { get; set; }

        [Display(Name = "Subtotal")]
        [NotMapped]
        public decimal subtotal => cantidad * precio_unitario;

    }
}

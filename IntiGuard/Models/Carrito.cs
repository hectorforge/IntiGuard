using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntiGuard.Models
{
    [Table("carrito")] // Maps to the 'carrito' table in the database
    public class Carrito
    {
        [Key] // Primary Key
        [Display(Name = "ID Carrito")]
        public int id_carrito { get; set; }

        [ForeignKey("Usuario")]
        [Display(Name = "ID Usuario")]
        public int id_usuario { get; set; }

        [ForeignKey("Producto")]
        [Display(Name = "ID Producto")]
        public int id_producto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        [Display(Name = "Cantidad")]
        public int cantidad { get; set; }

        [ForeignKey("CarritoCompra")]
        [Display(Name = "ID Carrito de Compra")]
        public int? id_carrito_compra { get; set; } 

    }
}

using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Carrito
    {
        [Display(Name = "ID Carrito")]
        public int id_carrito { get; set; }

        [Display(Name = "ID Usuario")]
        public int id_usuario { get; set; }

        [Display(Name = "ID Producto")]
        public int id_producto { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int cantidad { get; set; }
    }
}

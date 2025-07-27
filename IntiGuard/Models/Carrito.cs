using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntiGuard.Models
{

    public class Carrito
    {

        [Display(Name = "ID Carrito")]
        public int id_carrito { get; set; }


        [Display(Name = "ID Usuario")]
        public int id_usuario { get; set; }

        [Display(Name = "ID Producto")] public int id_producto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        [Display(Name = "Cantidad")]
        public int cantidad { get; set; }
        [Display(Name = "ID Carrito de Compra")] public int? id_carrito_compra { get; set; }

    }
}

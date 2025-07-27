using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntiGuard.Models
{
    
    public class CarritoCompra
    {
       
        [Display(Name = "ID Carrito Compra")] public int id { get; set; }

        [Display(Name = "Pagado")] public bool pagado { get; set; } = false; 

        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        [Display(Name = "cliente_id")]
        public int cliente_id { get; set; }

    }
}
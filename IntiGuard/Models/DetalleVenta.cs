using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class DetalleVenta
    {
        public int id_detalle_venta { get; set; }
        public int id_venta { get; set; }
        public int id_producto { get; set; }
        public Producto ? Producto { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal subtotal => cantidad * precio_unitario;
    }


}

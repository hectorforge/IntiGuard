using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class DetalleVenta
    {
        public int   IdDetalleVenta { get; set; }
        public int  IdVenta { get; set; }
        public int  IdProducto { get; set; }
        public string  NombreProducto { get; set; }
        public string? ImagenUrl { get; set; }
        public int  Cantidad { get; set; }
        public decimal  PrecioUnitario { get; set; }
    }
}

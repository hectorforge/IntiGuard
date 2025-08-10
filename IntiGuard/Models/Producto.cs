using System.ComponentModel.DataAnnotations.Schema;// For Decimal
using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Producto
    {
        public int  IdProducto  { get; set; }
        public string  NombreProducto { get; set; }
        public string  Descripcion { get; set; }
        public string  Marca { get; set; }
        public decimal  Precio { get; set; }
        public int  Stock { get; set; }
        public string  ImagenUrl { get; set; }
    }

}

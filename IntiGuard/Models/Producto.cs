using System.ComponentModel.DataAnnotations.Schema;// For Decimal
using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Producto
    {
        public int id_producto { get; set; }
        public string? nombre_producto { get; set; }
        public string? descripcion { get; set; }
        public string? marca { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }
        public string? imagen_url { get; set; }
    }

}

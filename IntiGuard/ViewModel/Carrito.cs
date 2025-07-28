using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntiGuard.ViewModel
{
    public class Carrito
    {
        public int id_carrito { get; set; }

        public int id_usuario { get; set; }

        public int id_producto { get; set; }

        public int cantidad { get; set; }
    }
}

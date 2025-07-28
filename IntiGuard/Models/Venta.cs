using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Venta
    {
        public int id_venta { get; set; }

        public virtual ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

        public int id_usuario { get; set; }
        public Usuario? Usuario { get; set; }
        public int id_comprobante { get; set; }
        public Comprobante? Comprobante { get; set; }
        public decimal total { get; set; }
        public DateTime fecha_venta { get; set; } = DateTime.Now;
    }
}

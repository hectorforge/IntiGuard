using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdUsuario { get; set; }
        public int IdComprobante { get; set; }
        public string Cliente { get; set; } // Para joins con usuario
        public string TipoComprobante { get; set; } // Para joins
        public string NumeroComprobante { get; set; } // Para joins
        public decimal Total { get; set; }
        public DateTime FechaVenta { get; set; }
    }
}

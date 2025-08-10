using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Comprobante
    {
        public int  IdComprobante { get; set; }
        public string  TipoComprobante { get; set; }
        public string  NumeroComprobante { get; set; }
    }
}

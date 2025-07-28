using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Comprobante
    {
        public int id_comprobante { get; set; }

        public string? tipo_comprobante { get; set; }
        public string? numero_comprobante { get; set; }
    }
}

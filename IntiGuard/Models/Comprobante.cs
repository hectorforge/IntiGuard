using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Comprobante
    {
        [Display(Name = "Id Comprobante")]
        public int id_comprobante { get; set; }

        [Display(Name = "Tipo Comprobante")]
        public string? tipo_comprobante { get; set; }

        [Display(Name = "Número Comprobante")]
        public string? numero_comprobante { get; set; }
    }
}

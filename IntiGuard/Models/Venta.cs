using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IntiGuard.Models
{
    public class Venta
    {
        [Display(Name = "Id Venta")] public int id_venta { get; set; }

        [Display(Name = "Id Detalle_Venta")] public int id_detalle_venta { get; set; }

        [Display(Name = "Id Usuario")] public int id_usuario { get; set; }

        [Display(Name = "Comprobante")] public int id_comprobante { get; set; }

        [Display(Name = "Total")]
        [Required(ErrorMessage = "El total es obligatorio.")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal total { get; set; }

        [Display(Name = "Fecha Venta")] public DateTime fecha_venta { get; set; } = DateTime.Now;

    }
}

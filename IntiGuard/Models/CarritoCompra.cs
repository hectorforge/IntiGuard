using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntiGuard.Models
{
    [Table("carrito_compra")] // Maps to the 'carrito_compra' table in the database
    public class CarritoCompra
    {
        [Key] // Primary Key
        [Column("id")] // Maps to the 'id' column
        [Display(Name = "ID Carrito Compra")]
        public int id { get; set; }

        [Column("pagado")] // Maps to the 'pagado' column
        [Display(Name = "Pagado")]
        public bool pagado { get; set; } = false; // Default value as in SQL

        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        [ForeignKey("cliente_id")]
        public int cliente_id { get; set; }

    }
}
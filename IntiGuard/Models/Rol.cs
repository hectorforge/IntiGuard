using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntiGuard.Models
{
    public class Rol
    {
        public int id_rol { get; set; }

        public string? nombre_rol { get; set; }
    }
}



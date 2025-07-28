using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntiGuard.Models
{
    public class Usuario
    {

        public int id_usuario { get; set; }


        public string? nombres { get; set; }

        public string? apellidos { get; set; }

        public string? correo { get; set; }

        public string? clave { get; set; }

        public int id_rol { get; set; }
        public DateTime fecha_registro { get; set; } = DateTime.Now;
    }

}


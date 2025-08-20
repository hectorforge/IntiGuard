using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntiGuard.Models
{
    public class Usuario
    {

        public int?   IdUsuario { get; set; }
        public string?  Nombres { get; set; }
        public string?  Apellidos { get; set; }
        public string?  Correo { get; set; }

        // Mas campos para el registro
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Foto { get; set; }

        public string?  Clave { get; set; }
        public int?   IdRol { get; set; }
        public string?  NombreRol { get; set; } // Para joins
        public DateTime?  FechaRegistro { get; set; }
    }

}


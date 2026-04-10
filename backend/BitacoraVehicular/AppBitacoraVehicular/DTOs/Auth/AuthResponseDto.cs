using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DateTime? Expiracion { get; set; }

        public int? UsuarioId { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Rol { get; set; }
        public string? Estado { get; set; }
    }
}

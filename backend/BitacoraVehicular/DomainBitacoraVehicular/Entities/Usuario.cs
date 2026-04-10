using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainBitacoraVehicular.Entities
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public int RolId { get; set; }
        public int EstadoId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public Rol? Rol { get; set; }
        public Estado? Estado { get; set; }

        public ICollection<AsignacionVehiculo> AsignacionesVehiculo { get; set; } = new List<AsignacionVehiculo>();
        public ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
        public ICollection<CargaCombustible> CargasCombustible { get; set; } = new List<CargaCombustible>();
    }
}

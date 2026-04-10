using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.AsignacionVehiculo
{
    public class AsignacionVehiculoDto
    {
        public int AsignacionVehiculoId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreCompletoUsuario { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;

        public int VehiculoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string VehiculoDescripcion { get; set; } = string.Empty;

        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = string.Empty;

        public DateTime FechaAsignacion { get; set; }
        public DateTime? FechaTermino { get; set; }
        public string? Observacion { get; set; }
    }
}

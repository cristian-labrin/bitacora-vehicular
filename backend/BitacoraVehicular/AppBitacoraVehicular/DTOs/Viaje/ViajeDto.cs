using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Viaje
{
    public class ViajeDto
    {
        public int ViajeId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public int VehiculoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string VehiculoDescripcion { get; set; } = string.Empty;
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = string.Empty;
        public DateTime FechaHoraSalida { get; set; }
        public DateTime? FechaHoraLlegada { get; set; }
        public int KilometrajeSalida { get; set; }
        public int? KilometrajeLlegada { get; set; }
        public string? ObservacionSalida { get; set; }
        public string? ObservacionLlegada { get; set; }
    }
}

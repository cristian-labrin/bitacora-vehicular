using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Dashboard
{
    public class ViajeAbiertoDashboardDto
    {
        public int ViajeId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public int VehiculoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string VehiculoDescripcion { get; set; } = string.Empty;
        public DateTime FechaHoraSalida { get; set; }
        public int KilometrajeSalida { get; set; }
        public string? ObservacionSalida { get; set; }
    }
}

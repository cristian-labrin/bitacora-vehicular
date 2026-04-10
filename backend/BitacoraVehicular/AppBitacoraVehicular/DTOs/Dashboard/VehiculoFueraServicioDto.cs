using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Dashboard
{
    public class VehiculoFueraServicioDto
    {
        public int VehiculoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string VehiculoDescripcion { get; set; } = string.Empty;
        public string EstadoNombre { get; set; } = string.Empty;
        public int KilometrajeActual { get; set; }
        public string? Observacion { get; set; }
    }
}

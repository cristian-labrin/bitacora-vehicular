using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Dashboard
{
    public class VehiculoProximoMantencionDto
    {
        public int VehiculoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string VehiculoDescripcion { get; set; } = string.Empty;
        public int KilometrajeActual { get; set; }
        public int? ProximaMantencionKm { get; set; }
        public DateTime? ProximaMantencionFecha { get; set; }
        public string? TipoMantencionNombre { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Mantencion
{
    public class MantencionDto
    {
        public int MantencionId { get; set; }
        public int VehiculoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string VehiculoDescripcion { get; set; } = string.Empty;
        public int TipoMantencionId { get; set; }
        public string TipoMantencionNombre { get; set; } = string.Empty;
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaSalida { get; set; }
        public int KilometrajeVehiculo { get; set; }
        public string Detalle { get; set; } = string.Empty;
        public decimal? Costo { get; set; }
        public int? ProximaMantencionKm { get; set; }
        public DateTime? ProximaMantencionFecha { get; set; }
    }
}

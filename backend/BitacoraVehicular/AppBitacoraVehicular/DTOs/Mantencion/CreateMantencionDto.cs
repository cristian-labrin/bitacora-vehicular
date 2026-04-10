using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Mantencion
{
    public class CreateMantencionDto
    {
        public int VehiculoId { get; set; }
        public int TipoMantencionId { get; set; }
        public int KilometrajeVehiculo { get; set; }
        public string Detalle { get; set; } = string.Empty;
        public decimal? Costo { get; set; }
        public int? ProximaMantencionKm { get; set; }
        public DateTime? ProximaMantencionFecha { get; set; }
    }
}

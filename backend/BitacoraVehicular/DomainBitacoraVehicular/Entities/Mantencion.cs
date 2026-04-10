using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainBitacoraVehicular.Entities
{
    public class Mantencion
    {
        public int MantencionId { get; set; }
        public int VehiculoId { get; set; }
        public int TipoMantencionId { get; set; }
        public int EstadoId { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaSalida { get; set; }
        public int KilometrajeVehiculo { get; set; }
        public string Detalle { get; set; } = string.Empty;
        public decimal? Costo { get; set; }
        public int? ProximaMantencionKm { get; set; }
        public DateTime? ProximaMantencionFecha { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public Vehiculo? Vehiculo { get; set; }
        public TipoMantencion? TipoMantencion { get; set; }
        public Estado? Estado { get; set; }
    }
}

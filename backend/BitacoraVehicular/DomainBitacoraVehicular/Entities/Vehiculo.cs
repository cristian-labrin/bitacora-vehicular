using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainBitacoraVehicular.Entities
{
    public class Vehiculo
    {
        public int VehiculoId { get; set; }
        public int ModeloId { get; set; }
        public int EstadoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Color { get; set; } = string.Empty;
        public int KilometrajeActual { get; set; }
        public string? Observacion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public Modelo? Modelo { get; set; }
        public Estado? Estado { get; set; }

        public ICollection<AsignacionVehiculo> AsignacionesVehiculo { get; set; } = new List<AsignacionVehiculo>();
        public ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
        public ICollection<CargaCombustible> CargasCombustible { get; set; } = new List<CargaCombustible>();
        public ICollection<Mantencion> Mantenciones { get; set; } = new List<Mantencion>();
    }
}

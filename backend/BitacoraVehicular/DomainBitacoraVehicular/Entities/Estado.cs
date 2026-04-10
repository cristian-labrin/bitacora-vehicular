using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainBitacoraVehicular.Entities
{
    public class Estado
    {
        public int EstadoId { get; set; }
        public int ContextoEstadoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ContextoEstado? ContextoEstado { get; set; }

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
        public ICollection<AsignacionVehiculo> AsignacionesVehiculo { get; set; } = new List<AsignacionVehiculo>();
        public ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
        public ICollection<Mantencion> Mantenciones { get; set; } = new List<Mantencion>();
    }
}

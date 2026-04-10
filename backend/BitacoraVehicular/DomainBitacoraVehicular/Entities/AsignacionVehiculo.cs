using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainBitacoraVehicular.Entities
{
    public class AsignacionVehiculo
    {
        public int AsignacionVehiculoId { get; set; }
        public int UsuarioId { get; set; }
        public int VehiculoId { get; set; }
        public int EstadoId { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? FechaTermino { get; set; }
        public string? Observacion { get; set; }
        public DateTime FechaCreacion { get; set; }

        public Usuario? Usuario { get; set; }
        public Vehiculo? Vehiculo { get; set; }
        public Estado? Estado { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainBitacoraVehicular.Entities
{
    public class Viaje
    {
        public int ViajeId { get; set; }
        public int UsuarioId { get; set; }
        public int VehiculoId { get; set; }
        public int EstadoId { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public DateTime? FechaHoraLlegada { get; set; }
        public int KilometrajeSalida { get; set; }
        public int? KilometrajeLlegada { get; set; }
        public string? ObservacionSalida { get; set; }
        public string? ObservacionLlegada { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public Usuario? Usuario { get; set; }
        public Vehiculo? Vehiculo { get; set; }
        public Estado? Estado { get; set; }

        public ICollection<CargaCombustible> CargasCombustible { get; set; } = new List<CargaCombustible>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.MisVehiculos
{
    public class MisVehiculosDto
    {
        public int VehiculoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string MarcaNombre { get; set; } = string.Empty;
        public string ModeloNombre { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Color { get; set; } = string.Empty;
        public int KilometrajeActual { get; set; }
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = string.Empty;
        public DateTime FechaAsignacion { get; set; }
        public string? ObservacionAsignacion { get; set; }
        public bool TieneViajeAbierto { get; set; }
        public int? ViajeAbiertoId { get; set; }
    }
}

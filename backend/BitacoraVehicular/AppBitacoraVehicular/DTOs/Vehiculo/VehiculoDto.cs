using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Vehiculo
{
    public class VehiculoDto
    {
        public int VehiculoId { get; set; }
        public int ModeloId { get; set; }
        public string MarcaNombre { get; set; } = string.Empty;
        public string ModeloNombre { get; set; } = string.Empty;
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = string.Empty;
        public string Patente { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Color { get; set; } = string.Empty;
        public int KilometrajeActual { get; set; }
        public string? Observacion { get; set; }
        public bool Activo { get; set; }
    }
}

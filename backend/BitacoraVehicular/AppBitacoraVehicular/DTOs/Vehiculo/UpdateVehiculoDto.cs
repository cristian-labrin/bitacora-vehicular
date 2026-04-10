using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Vehiculo
{
    public class UpdateVehiculoDto
    {
        public int ModeloId { get; set; }
        public int EstadoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Color { get; set; } = string.Empty;
        public int KilometrajeActual { get; set; }
        public string? Observacion { get; set; }
        public bool Activo { get; set; }
    }
}

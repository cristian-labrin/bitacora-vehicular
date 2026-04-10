using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.AsignacionVehiculo
{
    public class CreateAsignacionVehiculoDto
    {
        public int UsuarioId { get; set; }
        public int VehiculoId { get; set; }
        public string? Observacion { get; set; }
    }
}

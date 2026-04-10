using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Viaje
{
    public class CreateViajeDto
    {
        public int VehiculoId { get; set; }
        public string? ObservacionSalida { get; set; }
    }
}

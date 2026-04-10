using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Mantencion
{
    public class CerrarMantencionDto
    {
        public decimal? Costo { get; set; }
        public int? ProximaMantencionKm { get; set; }
        public DateTime? ProximaMantencionFecha { get; set; }
    }
}

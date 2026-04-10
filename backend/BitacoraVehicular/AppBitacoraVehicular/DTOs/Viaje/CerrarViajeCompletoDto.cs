using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Viaje
{
    public class CerrarViajeCompletoDto
    {
        public int KilometrajeLlegada { get; set; }
        public string? ObservacionLlegada { get; set; }

        public bool CargoCombustible { get; set; }

        public int? TipoCombustibleId { get; set; }
        public decimal? Litros { get; set; }
        public decimal? MontoCargado { get; set; }
        public string? NombreArchivoBoleta { get; set; }
        public string? RutaArchivoBoleta { get; set; }
        public string? ObservacionCombustible { get; set; }
    }
}

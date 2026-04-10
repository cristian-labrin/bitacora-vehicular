using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.CargaCombustible
{
    public class CreateCargaCombustibleDto
    {
        public int ViajeId { get; set; }
        public int TipoCombustibleId { get; set; }
        public decimal? Litros { get; set; }
        public decimal MontoCargado { get; set; }
        public string? NombreArchivoBoleta { get; set; }
        public string? RutaArchivoBoleta { get; set; }
        public string? Observacion { get; set; }
    }
}

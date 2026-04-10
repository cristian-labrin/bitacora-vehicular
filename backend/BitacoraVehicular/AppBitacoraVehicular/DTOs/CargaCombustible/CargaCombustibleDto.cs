using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.CargaCombustible
{
    public class CargaCombustibleDto
    {
        public int CargaCombustibleId { get; set; }
        public int ViajeId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public int VehiculoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string VehiculoDescripcion { get; set; } = string.Empty;
        public int TipoCombustibleId { get; set; }
        public string TipoCombustibleNombre { get; set; } = string.Empty;
        public decimal? Litros { get; set; }
        public decimal MontoCargado { get; set; }
        public DateTime FechaCarga { get; set; }
        public string? NombreArchivoBoleta { get; set; }
        public string? RutaArchivoBoleta { get; set; }
        public string? Observacion { get; set; }
    }
}

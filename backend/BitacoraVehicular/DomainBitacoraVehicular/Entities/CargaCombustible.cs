using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainBitacoraVehicular.Entities
{
    public class CargaCombustible
    {
        public int CargaCombustibleId { get; set; }
        public int ViajeId { get; set; }
        public int UsuarioId { get; set; }
        public int VehiculoId { get; set; }
        public int TipoCombustibleId { get; set; }
        public decimal? Litros { get; set; }
        public decimal MontoCargado { get; set; }
        public DateTime FechaCarga { get; set; }
        public string? NombreArchivoBoleta { get; set; }
        public string? RutaArchivoBoleta { get; set; }
        public string? Observacion { get; set; }
        public DateTime FechaCreacion { get; set; }

        public Viaje? Viaje { get; set; }
        public Usuario? Usuario { get; set; }
        public Vehiculo? Vehiculo { get; set; }
        public TipoCombustible? TipoCombustible { get; set; }
    }
}

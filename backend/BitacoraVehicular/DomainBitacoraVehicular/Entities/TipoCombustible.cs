using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainBitacoraVehicular.Entities
{
    public class TipoCombustible
    {
        public int TipoCombustibleId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ICollection<CargaCombustible> CargasCombustible { get; set; } = new List<CargaCombustible>();
    }
}

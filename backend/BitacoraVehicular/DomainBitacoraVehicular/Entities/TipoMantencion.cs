using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainBitacoraVehicular.Entities
{
    public class TipoMantencion
    {
        public int TipoMantencionId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ICollection<Mantencion> Mantenciones { get; set; } = new List<Mantencion>();
    }
}

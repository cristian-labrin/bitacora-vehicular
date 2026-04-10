using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Modelo
{
    public class ModeloDto
    {
        public int ModeloId { get; set; }
        public int MarcaId { get; set; }
        public string MarcaNombre { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}

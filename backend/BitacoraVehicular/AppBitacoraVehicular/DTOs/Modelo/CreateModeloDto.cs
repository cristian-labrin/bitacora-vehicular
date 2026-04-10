using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Modelo
{
    public class CreateModeloDto
    {
        public int MarcaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}

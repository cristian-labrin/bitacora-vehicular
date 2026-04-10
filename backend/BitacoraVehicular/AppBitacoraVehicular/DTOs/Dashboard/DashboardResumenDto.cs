using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.DTOs.Dashboard
{
    public class DashboardResumenDto
    {
        public int TotalVehiculos { get; set; }
        public int VehiculosDisponibles { get; set; }
        public int VehiculosEnUso { get; set; }
        public int VehiculosNoDisponibles { get; set; }
        public int ViajesAbiertos { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosPendientes { get; set; }
        public int AsignacionesActivas { get; set; }
    }
}

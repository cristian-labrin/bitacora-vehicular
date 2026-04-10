using AppBitacoraVehicular.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResumenDto> GetResumenAsync();
        Task<List<ViajeAbiertoDashboardDto>> GetViajesAbiertosAsync();
        Task<List<UsuarioPendienteDashboardDto>> GetUsuariosPendientesAsync();
        Task<List<VehiculoProximoMantencionDto>> GetVehiculosProximosMantencionAsync();
        Task<List<VehiculoFueraServicioDto>> GetVehiculosFueraServicioAsync();
    }
}

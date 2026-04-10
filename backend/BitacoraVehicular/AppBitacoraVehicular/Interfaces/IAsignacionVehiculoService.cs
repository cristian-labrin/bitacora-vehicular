using AppBitacoraVehicular.DTOs.AsignacionVehiculo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.Interfaces
{
    public interface IAsignacionVehiculoService
    {
        Task<List<AsignacionVehiculoDto>> GetAllAsync();
        Task<AsignacionVehiculoDto?> GetByIdAsync(int id);
        Task<AsignacionVehiculoDto> CreateAsync(CreateAsignacionVehiculoDto request);
        Task<bool> FinalizarAsync(int id, FinalizarAsignacionVehiculoDto request);
    }
}

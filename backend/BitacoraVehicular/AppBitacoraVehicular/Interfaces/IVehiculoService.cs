using AppBitacoraVehicular.DTOs.Vehiculo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.Interfaces
{
    public interface IVehiculoService
    {
        Task<List<VehiculoDto>> GetAllAsync();
        Task<VehiculoDto?> GetByIdAsync(int id);
        Task<VehiculoDto> CreateAsync(CreateVehiculoDto request);
        Task<bool> UpdateAsync(int id, UpdateVehiculoDto request);
        Task<bool> DeleteAsync(int id);
    }
}

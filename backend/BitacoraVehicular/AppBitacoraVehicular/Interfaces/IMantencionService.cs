using AppBitacoraVehicular.DTOs.Mantencion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.Interfaces
{
    public interface IMantencionService
    {
        Task<List<MantencionDto>> GetAllAsync();
        Task<MantencionDto?> GetByIdAsync(int id);
        Task<MantencionDto> CreateAsync(CreateMantencionDto request);
        Task<bool> CerrarAsync(int id, CerrarMantencionDto request);
        Task<List<MantencionDto>> GetProximasAsync();
    }
}

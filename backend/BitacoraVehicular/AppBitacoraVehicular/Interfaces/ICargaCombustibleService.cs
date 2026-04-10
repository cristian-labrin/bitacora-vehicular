using AppBitacoraVehicular.DTOs.CargaCombustible;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.Interfaces
{
    public interface ICargaCombustibleService
    {
        Task<List<CargaCombustibleDto>> GetMisCargasAsync(int usuarioId);
        Task<List<CargaCombustibleDto>> GetAllAsync();
        Task<CargaCombustibleDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin);
        Task<CargaCombustibleDto> CreateAsync(int usuarioId, CreateCargaCombustibleDto request);
    }
}

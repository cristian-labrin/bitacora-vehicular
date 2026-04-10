using AppBitacoraVehicular.DTOs.Marca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.Interfaces
{
    public interface IMarcaService
    {
        Task<List<MarcaDto>> GetAllAsync();
        Task<MarcaDto?> GetByIdAsync(int id);
        Task<MarcaDto> CreateAsync(CreateMarcaDto request);
        Task<bool> UpdateAsync(int id, UpdateMarcaDto request);
        Task<bool> DeleteAsync(int id);
    }
}

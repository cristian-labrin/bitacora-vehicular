using AppBitacoraVehicular.DTOs.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.Interfaces
{
    public interface IModeloService
    {
        Task<List<ModeloDto>> GetAllAsync();
        Task<ModeloDto?> GetByIdAsync(int id);
        Task<ModeloDto> CreateAsync(CreateModeloDto request);
        Task<bool> UpdateAsync(int id, UpdateModeloDto request);
        Task<bool> DeleteAsync(int id);
    }
}

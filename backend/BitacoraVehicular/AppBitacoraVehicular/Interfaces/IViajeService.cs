using AppBitacoraVehicular.DTOs.Viaje;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.Interfaces
{
    public interface IViajeService
    {
        Task<List<ViajeDto>> GetMisViajesAsync(int usuarioId);
        Task<List<ViajeDto>> GetViajesAbiertosAsync();
        Task<ViajeDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin);
        Task<ViajeDto> IniciarViajeAsync(int usuarioId, CreateViajeDto request);
        Task<bool> CerrarViajeAsync(int viajeId, int usuarioId, bool esAdmin, CerrarViajeDto request);
        Task<bool> CerrarViajeCompletoAsync(int viajeId, int usuarioId, bool esAdmin, CerrarViajeCompletoDto request);
        Task<bool> ForzarCierreAsync(int viajeId, ForzarCierreViajeDto request);
    }
}

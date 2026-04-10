using AppBitacoraVehicular.DTOs.MisVehiculos;

namespace AppBitacoraVehicular.Interfaces
{
    public interface IMisVehiculosService
    {
        Task<List<MisVehiculosDto>> GetMisVehiculosAsync(int usuarioId);
    }
}

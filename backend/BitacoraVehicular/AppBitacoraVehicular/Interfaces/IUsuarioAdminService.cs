using AppBitacoraVehicular.DTOs.UsuarioAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBitacoraVehicular.Interfaces
{
    public interface IUsuarioAdminService
    {
        Task<List<UsuarioAdminDto>> GetAllAsync();
        Task<List<UsuarioAdminDto>> GetPendientesAsync();
        Task<UsuarioAdminDto?> GetByIdAsync(int id);
        Task<bool> ActivarAsync(int id);
        Task<bool> InactivarAsync(int id);
    }
}

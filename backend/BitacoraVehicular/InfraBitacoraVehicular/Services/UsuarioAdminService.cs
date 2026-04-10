using AppBitacoraVehicular.DTOs.UsuarioAdmin;
using AppBitacoraVehicular.Interfaces;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfraBitacoraVehicular.Services
{
    public class UsuarioAdminService : IUsuarioAdminService
    {
        private readonly BitacoraVehicularDbContext _context;

        public UsuarioAdminService(BitacoraVehicularDbContext context)
        {
            _context = context;
        }

        public async Task<List<UsuarioAdminDto>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Estado)
                .OrderByDescending(u => u.FechaCreacion)
                .Select(MapDto())
                .ToListAsync();
        }

        public async Task<List<UsuarioAdminDto>> GetPendientesAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Estado)
                .Where(u => u.Estado != null && u.Estado.Nombre == "Pendiente")
                .OrderByDescending(u => u.FechaCreacion)
                .Select(MapDto())
                .ToListAsync();
        }

        public async Task<UsuarioAdminDto?> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Estado)
                .Where(u => u.UsuarioId == id)
                .Select(MapDto())
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ActivarAsync(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null)
                return false;

            var contextoUsuario = await _context.ContextosEstado
                .FirstOrDefaultAsync(c => c.Nombre == "USUARIO");

            if (contextoUsuario is null)
                throw new InvalidOperationException("No existe el contexto de estados de usuario.");

            var estadoActivo = await _context.Estados
                .FirstOrDefaultAsync(e =>
                    e.ContextoEstadoId == contextoUsuario.ContextoEstadoId &&
                    e.Nombre == "Activo");

            if (estadoActivo is null)
                throw new InvalidOperationException("No existe el estado 'Activo' para usuarios.");

            usuario.EstadoId = estadoActivo.EstadoId;
            usuario.Activo = true;
            usuario.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InactivarAsync(int id)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario is null)
                return false;

            var contextoUsuario = await _context.ContextosEstado
                .FirstOrDefaultAsync(c => c.Nombre == "USUARIO");

            if (contextoUsuario is null)
                throw new InvalidOperationException("No existe el contexto de estados de usuario.");

            var estadoInactivo = await _context.Estados
                .FirstOrDefaultAsync(e =>
                    e.ContextoEstadoId == contextoUsuario.ContextoEstadoId &&
                    e.Nombre == "Inactivo");

            if (estadoInactivo is null)
                throw new InvalidOperationException("No existe el estado 'Inactivo' para usuarios.");

            usuario.EstadoId = estadoInactivo.EstadoId;
            usuario.Activo = false;
            usuario.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private static System.Linq.Expressions.Expression<Func<DomainBitacoraVehicular.Entities.Usuario, UsuarioAdminDto>> MapDto()
        {
            return u => new UsuarioAdminDto
            {
                UsuarioId = u.UsuarioId,
                NombreCompleto = u.NombreCompleto,
                Correo = u.Correo,
                NombreUsuario = u.NombreUsuario,
                RolId = u.RolId,
                RolNombre = u.Rol != null ? u.Rol.Nombre : string.Empty,
                EstadoId = u.EstadoId,
                EstadoNombre = u.Estado != null ? u.Estado.Nombre : string.Empty,
                Activo = u.Activo,
                FechaCreacion = u.FechaCreacion,
                FechaActualizacion = u.FechaActualizacion
            };
        }
    }
}

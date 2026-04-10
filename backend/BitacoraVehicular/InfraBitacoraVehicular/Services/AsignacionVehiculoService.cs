using AppBitacoraVehicular.DTOs.AsignacionVehiculo;
using AppBitacoraVehicular.Interfaces;
using DomainBitacoraVehicular.Entities;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfraBitacoraVehicular.Services
{
    public class AsignacionVehiculoService : IAsignacionVehiculoService
    {
        private readonly BitacoraVehicularDbContext _context;

        public AsignacionVehiculoService(BitacoraVehicularDbContext context)
        {
            _context = context;
        }

        public async Task<List<AsignacionVehiculoDto>> GetAllAsync()
        {
            return await _context.AsignacionesVehiculo
                .Include(a => a.Usuario)
                .Include(a => a.Vehiculo)
                    .ThenInclude(v => v!.Modelo)
                        .ThenInclude(m => m!.Marca)
                .Include(a => a.Estado)
                .OrderByDescending(a => a.FechaAsignacion)
                .Select(a => new AsignacionVehiculoDto
                {
                    AsignacionVehiculoId = a.AsignacionVehiculoId,
                    UsuarioId = a.UsuarioId,
                    NombreCompletoUsuario = a.Usuario != null ? a.Usuario.NombreCompleto : string.Empty,
                    NombreUsuario = a.Usuario != null ? a.Usuario.NombreUsuario : string.Empty,
                    VehiculoId = a.VehiculoId,
                    Patente = a.Vehiculo != null ? a.Vehiculo.Patente : string.Empty,
                    VehiculoDescripcion = a.Vehiculo != null && a.Vehiculo.Modelo != null && a.Vehiculo.Modelo.Marca != null
                        ? $"{a.Vehiculo.Modelo.Marca.Nombre} {a.Vehiculo.Modelo.Nombre}"
                        : string.Empty,
                    EstadoId = a.EstadoId,
                    EstadoNombre = a.Estado != null ? a.Estado.Nombre : string.Empty,
                    FechaAsignacion = a.FechaAsignacion,
                    FechaTermino = a.FechaTermino,
                    Observacion = a.Observacion
                })
                .ToListAsync();
        }

        public async Task<AsignacionVehiculoDto?> GetByIdAsync(int id)
        {
            return await _context.AsignacionesVehiculo
                .Include(a => a.Usuario)
                .Include(a => a.Vehiculo)
                    .ThenInclude(v => v!.Modelo)
                        .ThenInclude(m => m!.Marca)
                .Include(a => a.Estado)
                .Where(a => a.AsignacionVehiculoId == id)
                .Select(a => new AsignacionVehiculoDto
                {
                    AsignacionVehiculoId = a.AsignacionVehiculoId,
                    UsuarioId = a.UsuarioId,
                    NombreCompletoUsuario = a.Usuario != null ? a.Usuario.NombreCompleto : string.Empty,
                    NombreUsuario = a.Usuario != null ? a.Usuario.NombreUsuario : string.Empty,
                    VehiculoId = a.VehiculoId,
                    Patente = a.Vehiculo != null ? a.Vehiculo.Patente : string.Empty,
                    VehiculoDescripcion = a.Vehiculo != null && a.Vehiculo.Modelo != null && a.Vehiculo.Modelo.Marca != null
                        ? $"{a.Vehiculo.Modelo.Marca.Nombre} {a.Vehiculo.Modelo.Nombre}"
                        : string.Empty,
                    EstadoId = a.EstadoId,
                    EstadoNombre = a.Estado != null ? a.Estado.Nombre : string.Empty,
                    FechaAsignacion = a.FechaAsignacion,
                    FechaTermino = a.FechaTermino,
                    Observacion = a.Observacion
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AsignacionVehiculoDto> CreateAsync(CreateAsignacionVehiculoDto request)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Estado)
                .FirstOrDefaultAsync(u => u.UsuarioId == request.UsuarioId);

            if (usuario is null)
                throw new InvalidOperationException("El usuario no existe.");

            if (!usuario.Activo || !string.Equals(usuario.Estado?.Nombre, "Activo", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("El usuario no está habilitado para asignaciones.");

            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.VehiculoId == request.VehiculoId);

            if (vehiculo is null)
                throw new InvalidOperationException("El vehículo no existe.");

            if (!vehiculo.Activo)
                throw new InvalidOperationException("El vehículo está inactivo.");

            var existeAsignacionActivaVehiculo = await _context.AsignacionesVehiculo
                .Include(a => a.Estado)
                .AnyAsync(a =>
                    a.VehiculoId == request.VehiculoId &&
                    a.FechaTermino == null &&
                    a.Estado != null &&
                    a.Estado.Nombre == "Activa");

            if (existeAsignacionActivaVehiculo)
                throw new InvalidOperationException("El vehículo ya tiene una asignación activa.");

            var contextoAsignacion = await _context.ContextosEstado
                .FirstOrDefaultAsync(c => c.Nombre == "ASIGNACION");

            if (contextoAsignacion is null)
                throw new InvalidOperationException("No existe el contexto de asignación.");

            var estadoActiva = await _context.Estados
                .FirstOrDefaultAsync(e =>
                    e.ContextoEstadoId == contextoAsignacion.ContextoEstadoId &&
                    e.Nombre == "Activa");

            if (estadoActiva is null)
                throw new InvalidOperationException("No existe el estado 'Activa' para asignación.");

            var asignacion = new AsignacionVehiculo
            {
                UsuarioId = request.UsuarioId,
                VehiculoId = request.VehiculoId,
                EstadoId = estadoActiva.EstadoId,
                FechaAsignacion = DateTime.UtcNow,
                Observacion = request.Observacion?.Trim(),
                FechaCreacion = DateTime.UtcNow
            };

            _context.AsignacionesVehiculo.Add(asignacion);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(asignacion.AsignacionVehiculoId)
                ?? throw new InvalidOperationException("No se pudo recuperar la asignación creada.");
        }

        public async Task<bool> FinalizarAsync(int id, FinalizarAsignacionVehiculoDto request)
        {
            var asignacion = await _context.AsignacionesVehiculo
                .Include(a => a.Estado)
                .FirstOrDefaultAsync(a => a.AsignacionVehiculoId == id);

            if (asignacion is null)
                return false;

            if (asignacion.FechaTermino is not null)
                throw new InvalidOperationException("La asignación ya está finalizada.");

            var contextoAsignacion = await _context.ContextosEstado
                .FirstOrDefaultAsync(c => c.Nombre == "ASIGNACION");

            if (contextoAsignacion is null)
                throw new InvalidOperationException("No existe el contexto de asignación.");

            var estadoFinalizada = await _context.Estados
                .FirstOrDefaultAsync(e =>
                    e.ContextoEstadoId == contextoAsignacion.ContextoEstadoId &&
                    e.Nombre == "Finalizada");

            if (estadoFinalizada is null)
                throw new InvalidOperationException("No existe el estado 'Finalizada' para asignación.");

            asignacion.EstadoId = estadoFinalizada.EstadoId;
            asignacion.FechaTermino = DateTime.UtcNow;
            asignacion.Observacion = string.IsNullOrWhiteSpace(request.Observacion)
                ? asignacion.Observacion
                : request.Observacion.Trim();

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

using AppBitacoraVehicular.DTOs.Mantencion;
using AppBitacoraVehicular.Interfaces;
using DomainBitacoraVehicular.Entities;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfraBitacoraVehicular.Services
{
    public class MantencionService : IMantencionService
    {
        private readonly BitacoraVehicularDbContext _context;

        public MantencionService(BitacoraVehicularDbContext context)
        {
            _context = context;
        }

        public async Task<List<MantencionDto>> GetAllAsync()
        {
            return await _context.Mantenciones
                .Include(m => m.Vehiculo)
                    .ThenInclude(v => v!.Modelo)
                        .ThenInclude(mo => mo!.Marca)
                .Include(m => m.TipoMantencion)
                .Include(m => m.Estado)
                .OrderByDescending(m => m.FechaIngreso)
                .Select(MapDto())
                .ToListAsync();
        }

        public async Task<MantencionDto?> GetByIdAsync(int id)
        {
            return await _context.Mantenciones
                .Include(m => m.Vehiculo)
                    .ThenInclude(v => v!.Modelo)
                        .ThenInclude(mo => mo!.Marca)
                .Include(m => m.TipoMantencion)
                .Include(m => m.Estado)
                .Where(m => m.MantencionId == id)
                .Select(MapDto())
                .FirstOrDefaultAsync();
        }

        public async Task<MantencionDto> CreateAsync(CreateMantencionDto request)
        {
            var vehiculo = await _context.Vehiculos
                .Include(v => v.Estado)
                .FirstOrDefaultAsync(v => v.VehiculoId == request.VehiculoId);

            if (vehiculo is null)
                throw new InvalidOperationException("El vehículo no existe.");

            if (!vehiculo.Activo)
                throw new InvalidOperationException("El vehículo está inactivo.");

            var tipoExiste = await _context.TiposMantencion
                .AnyAsync(t => t.TipoMantencionId == request.TipoMantencionId && t.Activo);

            if (!tipoExiste)
                throw new InvalidOperationException("El tipo de mantención no existe o está inactivo.");

            var mantencionAbierta = await _context.Mantenciones
                .AnyAsync(m => m.VehiculoId == request.VehiculoId && m.FechaSalida == null);

            if (mantencionAbierta)
                throw new InvalidOperationException("El vehículo ya tiene una mantención abierta.");

            var contextoMantencion = await _context.ContextosEstado
                .FirstAsync(c => c.Nombre == "MANTENCION");

            var estadoPendiente = await _context.Estados
                .FirstAsync(e =>
                    e.ContextoEstadoId == contextoMantencion.ContextoEstadoId &&
                    e.Nombre == "Pendiente");

            var contextoVehiculo = await _context.ContextosEstado
                .FirstAsync(c => c.Nombre == "VEHICULO");

            var estadoVehiculoMantencion = await _context.Estados
                .FirstAsync(e =>
                    e.ContextoEstadoId == contextoVehiculo.ContextoEstadoId &&
                    e.Nombre == "Mantencion");

            var mantencion = new Mantencion
            {
                VehiculoId = request.VehiculoId,
                TipoMantencionId = request.TipoMantencionId,
                EstadoId = estadoPendiente.EstadoId,
                FechaIngreso = DateTime.UtcNow,
                KilometrajeVehiculo = request.KilometrajeVehiculo,
                Detalle = request.Detalle.Trim(),
                Costo = request.Costo,
                ProximaMantencionKm = request.ProximaMantencionKm,
                ProximaMantencionFecha = request.ProximaMantencionFecha,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Mantenciones.Add(mantencion);

            vehiculo.EstadoId = estadoVehiculoMantencion.EstadoId;
            vehiculo.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(mantencion.MantencionId)
                ?? throw new InvalidOperationException("No se pudo recuperar la mantención creada.");
        }

        public async Task<bool> CerrarAsync(int id, CerrarMantencionDto request)
        {
            var mantencion = await _context.Mantenciones
                .Include(m => m.Vehiculo)
                .FirstOrDefaultAsync(m => m.MantencionId == id);

            if (mantencion is null)
                return false;

            if (mantencion.FechaSalida != null)
                throw new InvalidOperationException("La mantención ya está finalizada.");

            var contextoMantencion = await _context.ContextosEstado
                .FirstAsync(c => c.Nombre == "MANTENCION");

            var estadoFinalizada = await _context.Estados
                .FirstAsync(e =>
                    e.ContextoEstadoId == contextoMantencion.ContextoEstadoId &&
                    e.Nombre == "Finalizada");

            var contextoVehiculo = await _context.ContextosEstado
                .FirstAsync(c => c.Nombre == "VEHICULO");

            var estadoDisponible = await _context.Estados
                .FirstAsync(e =>
                    e.ContextoEstadoId == contextoVehiculo.ContextoEstadoId &&
                    e.Nombre == "Disponible");

            mantencion.EstadoId = estadoFinalizada.EstadoId;
            mantencion.FechaSalida = DateTime.UtcNow;
            mantencion.Costo = request.Costo ?? mantencion.Costo;
            mantencion.ProximaMantencionKm = request.ProximaMantencionKm;
            mantencion.ProximaMantencionFecha = request.ProximaMantencionFecha;
            mantencion.FechaActualizacion = DateTime.UtcNow;

            if (mantencion.Vehiculo != null)
            {
                mantencion.Vehiculo.EstadoId = estadoDisponible.EstadoId;
                mantencion.Vehiculo.FechaActualizacion = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MantencionDto>> GetProximasAsync()
        {
            var hoy = DateTime.UtcNow.Date;

            return await _context.Mantenciones
                .Include(m => m.Vehiculo)
                    .ThenInclude(v => v!.Modelo)
                        .ThenInclude(mo => mo!.Marca)
                .Include(m => m.TipoMantencion)
                .Include(m => m.Estado)
                .Where(m =>
                    m.FechaSalida != null &&
                    (
                        (m.ProximaMantencionFecha.HasValue && m.ProximaMantencionFecha.Value.Date <= hoy.AddDays(30)) ||
                        (m.ProximaMantencionKm.HasValue && m.Vehiculo != null && m.Vehiculo.KilometrajeActual >= m.ProximaMantencionKm.Value - 500)
                    ))
                .OrderBy(m => m.ProximaMantencionFecha)
                .ThenBy(m => m.ProximaMantencionKm)
                .Select(MapDto())
                .ToListAsync();
        }

        private static System.Linq.Expressions.Expression<Func<Mantencion, MantencionDto>> MapDto()
        {
            return m => new MantencionDto
            {
                MantencionId = m.MantencionId,
                VehiculoId = m.VehiculoId,
                Patente = m.Vehiculo != null ? m.Vehiculo.Patente : string.Empty,
                VehiculoDescripcion = m.Vehiculo != null && m.Vehiculo.Modelo != null && m.Vehiculo.Modelo.Marca != null
                    ? m.Vehiculo.Modelo.Marca.Nombre + " " + m.Vehiculo.Modelo.Nombre
                    : string.Empty,
                TipoMantencionId = m.TipoMantencionId,
                TipoMantencionNombre = m.TipoMantencion != null ? m.TipoMantencion.Nombre : string.Empty,
                EstadoId = m.EstadoId,
                EstadoNombre = m.Estado != null ? m.Estado.Nombre : string.Empty,
                FechaIngreso = m.FechaIngreso,
                FechaSalida = m.FechaSalida,
                KilometrajeVehiculo = m.KilometrajeVehiculo,
                Detalle = m.Detalle,
                Costo = m.Costo,
                ProximaMantencionKm = m.ProximaMantencionKm,
                ProximaMantencionFecha = m.ProximaMantencionFecha
            };
        }
    }
}

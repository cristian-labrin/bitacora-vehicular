using AppBitacoraVehicular.DTOs.Viaje;
using AppBitacoraVehicular.Interfaces;
using DomainBitacoraVehicular.Entities;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfraBitacoraVehicular.Services
{
    public class ViajeService : IViajeService
    {
        private readonly BitacoraVehicularDbContext _context;

        public ViajeService(BitacoraVehicularDbContext context)
        {
            _context = context;
        }

        public async Task<List<ViajeDto>> GetMisViajesAsync(int usuarioId)
        {
            return await _context.Viajes
                .Include(v => v.Usuario)
                .Include(v => v.Vehiculo)
                    .ThenInclude(vh => vh!.Modelo)
                        .ThenInclude(m => m!.Marca)
                .Include(v => v.Estado)
                .Where(v => v.UsuarioId == usuarioId)
                .OrderByDescending(v => v.FechaHoraSalida)
                .Select(MapViajeDto())
                .ToListAsync();
        }

        public async Task<List<ViajeDto>> GetViajesAbiertosAsync()
        {
            return await _context.Viajes
                .Include(v => v.Usuario)
                .Include(v => v.Vehiculo)
                    .ThenInclude(vh => vh!.Modelo)
                        .ThenInclude(m => m!.Marca)
                .Include(v => v.Estado)
                .Where(v => v.FechaHoraLlegada == null)
                .OrderByDescending(v => v.FechaHoraSalida)
                .Select(MapViajeDto())
                .ToListAsync();
        }

        public async Task<ViajeDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.Viajes
                .Include(v => v.Usuario)
                .Include(v => v.Vehiculo)
                    .ThenInclude(vh => vh!.Modelo)
                        .ThenInclude(m => m!.Marca)
                .Include(v => v.Estado)
                .Where(v => v.ViajeId == id);

            if (!esAdmin)
                query = query.Where(v => v.UsuarioId == usuarioId);

            return await query
                .Select(MapViajeDto())
                .FirstOrDefaultAsync();
        }

        public async Task<ViajeDto> IniciarViajeAsync(int usuarioId, CreateViajeDto request)
        {
            var vehiculo = await _context.Vehiculos
                .Include(v => v.Estado)
                .FirstOrDefaultAsync(v => v.VehiculoId == request.VehiculoId);

            if (vehiculo is null)
                throw new InvalidOperationException("El vehículo no existe.");

            if (!vehiculo.Activo)
                throw new InvalidOperationException("El vehículo está inactivo.");

            if (vehiculo.Estado == null || vehiculo.Estado.Nombre != "Disponible")
                throw new InvalidOperationException("El vehículo no está disponible para iniciar viaje.");

            var tieneAsignacion = await _context.AsignacionesVehiculo
                .Include(a => a.Estado)
                .AnyAsync(a =>
                    a.UsuarioId == usuarioId &&
                    a.VehiculoId == request.VehiculoId &&
                    a.FechaTermino == null &&
                    a.Estado != null &&
                    a.Estado.Nombre == "Activa");

            if (!tieneAsignacion)
                throw new InvalidOperationException("El vehículo no está asignado al usuario.");

            var usuarioTieneViajeAbierto = await _context.Viajes
                .AnyAsync(v => v.UsuarioId == usuarioId && v.FechaHoraLlegada == null);

            if (usuarioTieneViajeAbierto)
                throw new InvalidOperationException("El usuario ya tiene un viaje abierto.");

            var vehiculoTieneViajeAbierto = await _context.Viajes
                .AnyAsync(v => v.VehiculoId == request.VehiculoId && v.FechaHoraLlegada == null);

            if (vehiculoTieneViajeAbierto)
                throw new InvalidOperationException("El vehículo ya tiene un viaje abierto.");

            var contextoViaje = await _context.ContextosEstado
                .FirstAsync(c => c.Nombre == "VIAJE");

            var estadoAbierto = await _context.Estados
                .FirstAsync(e =>
                    e.ContextoEstadoId == contextoViaje.ContextoEstadoId &&
                    e.Nombre == "Abierto");

            var viaje = new Viaje
            {
                UsuarioId = usuarioId,
                VehiculoId = request.VehiculoId,
                EstadoId = estadoAbierto.EstadoId,
                FechaHoraSalida = DateTime.UtcNow,
                KilometrajeSalida = vehiculo.KilometrajeActual,
                ObservacionSalida = request.ObservacionSalida?.Trim(),
                FechaCreacion = DateTime.UtcNow
            };

            _context.Viajes.Add(viaje);

            var contextoVehiculo = await _context.ContextosEstado
                .FirstAsync(c => c.Nombre == "VEHICULO");

            var estadoEnUso = await _context.Estados
                .FirstAsync(e =>
                    e.ContextoEstadoId == contextoVehiculo.ContextoEstadoId &&
                    e.Nombre == "EnUso");

            vehiculo.EstadoId = estadoEnUso.EstadoId;
            vehiculo.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(viaje.ViajeId, usuarioId, true)
                ?? throw new InvalidOperationException("No se pudo recuperar el viaje creado.");
        }

        public async Task<bool> CerrarViajeAsync(int viajeId, int usuarioId, bool esAdmin, CerrarViajeDto request)
        {
            var query = _context.Viajes
                .Include(v => v.Vehiculo)
                .Include(v => v.Estado)
                .Where(v => v.ViajeId == viajeId);

            if (!esAdmin)
                query = query.Where(v => v.UsuarioId == usuarioId);

            var viaje = await query.FirstOrDefaultAsync();

            if (viaje is null)
                return false;

            if (viaje.FechaHoraLlegada != null)
                throw new InvalidOperationException("El viaje ya está cerrado.");

            if (request.KilometrajeLlegada < viaje.KilometrajeSalida)
                throw new InvalidOperationException("El kilometraje final no puede ser menor al inicial.");

            var contextoViaje = await _context.ContextosEstado
                .FirstAsync(c => c.Nombre == "VIAJE");

            var estadoCerrado = await _context.Estados
                .FirstAsync(e =>
                    e.ContextoEstadoId == contextoViaje.ContextoEstadoId &&
                    e.Nombre == "Cerrado");

            viaje.KilometrajeLlegada = request.KilometrajeLlegada;
            viaje.FechaHoraLlegada = DateTime.UtcNow;
            viaje.ObservacionLlegada = request.ObservacionLlegada?.Trim();
            viaje.EstadoId = estadoCerrado.EstadoId;
            viaje.FechaActualizacion = DateTime.UtcNow;

            if (viaje.Vehiculo != null)
            {
                var contextoVehiculo = await _context.ContextosEstado
                    .FirstAsync(c => c.Nombre == "VEHICULO");

                var estadoDisponible = await _context.Estados
                    .FirstAsync(e =>
                        e.ContextoEstadoId == contextoVehiculo.ContextoEstadoId &&
                        e.Nombre == "Disponible");

                viaje.Vehiculo.KilometrajeActual = request.KilometrajeLlegada;
                viaje.Vehiculo.EstadoId = estadoDisponible.EstadoId;
                viaje.Vehiculo.FechaActualizacion = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CerrarViajeCompletoAsync(int viajeId, int usuarioId, bool esAdmin, CerrarViajeCompletoDto request)
        {
            var query = _context.Viajes
                .Include(v => v.Vehiculo)
                .Include(v => v.Estado)
                .Where(v => v.ViajeId == viajeId);

            if (!esAdmin)
                query = query.Where(v => v.UsuarioId == usuarioId);

            var viaje = await query.FirstOrDefaultAsync();

            if (viaje is null)
                return false;

            if (viaje.FechaHoraLlegada != null)
                throw new InvalidOperationException("El viaje ya está cerrado.");

            if (request.KilometrajeLlegada < viaje.KilometrajeSalida)
                throw new InvalidOperationException("El kilometraje final no puede ser menor al inicial.");

            if (request.CargoCombustible)
            {
                if (!request.MontoCargado.HasValue || request.MontoCargado.Value <= 0)
                    throw new InvalidOperationException("Si cargó combustible, el monto cargado es obligatorio y debe ser mayor a cero.");

                if (!request.TipoCombustibleId.HasValue)
                    throw new InvalidOperationException("Si cargó combustible, el tipo de combustible es obligatorio.");

                var tipoCombustibleExiste = await _context.TiposCombustible
                    .AnyAsync(t => t.TipoCombustibleId == request.TipoCombustibleId.Value && t.Activo);

                if (!tipoCombustibleExiste)
                    throw new InvalidOperationException("El tipo de combustible no existe o está inactivo.");

                var yaExisteCarga = await _context.CargasCombustible
                    .AnyAsync(c => c.ViajeId == viajeId);

                if (yaExisteCarga)
                    throw new InvalidOperationException("El viaje ya tiene una carga de combustible registrada.");
            }

            var contextoViaje = await _context.ContextosEstado
                .FirstAsync(c => c.Nombre == "VIAJE");

            var estadoCerrado = await _context.Estados
                .FirstAsync(e =>
                    e.ContextoEstadoId == contextoViaje.ContextoEstadoId &&
                    e.Nombre == "Cerrado");

            viaje.KilometrajeLlegada = request.KilometrajeLlegada;
            viaje.FechaHoraLlegada = DateTime.UtcNow;
            viaje.ObservacionLlegada = request.ObservacionLlegada?.Trim();
            viaje.EstadoId = estadoCerrado.EstadoId;
            viaje.FechaActualizacion = DateTime.UtcNow;

            if (viaje.Vehiculo != null)
            {
                var contextoVehiculo = await _context.ContextosEstado
                    .FirstAsync(c => c.Nombre == "VEHICULO");

                var estadoDisponible = await _context.Estados
                    .FirstAsync(e =>
                        e.ContextoEstadoId == contextoVehiculo.ContextoEstadoId &&
                        e.Nombre == "Disponible");

                viaje.Vehiculo.KilometrajeActual = request.KilometrajeLlegada;
                viaje.Vehiculo.EstadoId = estadoDisponible.EstadoId;
                viaje.Vehiculo.FechaActualizacion = DateTime.UtcNow;
            }

            if (request.CargoCombustible)
            {
                var carga = new DomainBitacoraVehicular.Entities.CargaCombustible
                {
                    ViajeId = viaje.ViajeId,
                    UsuarioId = viaje.UsuarioId,
                    VehiculoId = viaje.VehiculoId,
                    TipoCombustibleId = request.TipoCombustibleId!.Value,
                    Litros = request.Litros,
                    MontoCargado = request.MontoCargado!.Value,
                    FechaCarga = DateTime.UtcNow,
                    NombreArchivoBoleta = request.NombreArchivoBoleta?.Trim(),
                    RutaArchivoBoleta = request.RutaArchivoBoleta?.Trim(),
                    Observacion = request.ObservacionCombustible?.Trim(),
                    FechaCreacion = DateTime.UtcNow
                };

                _context.CargasCombustible.Add(carga);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ForzarCierreAsync(int viajeId, ForzarCierreViajeDto request)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Vehiculo)
                .FirstOrDefaultAsync(v => v.ViajeId == viajeId);

            if (viaje is null)
                return false;

            if (viaje.FechaHoraLlegada != null)
                throw new InvalidOperationException("El viaje ya está cerrado.");

            if (request.KilometrajeLlegada < viaje.KilometrajeSalida)
                throw new InvalidOperationException("El kilometraje final no puede ser menor al inicial.");

            var contextoViaje = await _context.ContextosEstado
                .FirstAsync(c => c.Nombre == "VIAJE");

            var estadoCerrado = await _context.Estados
                .FirstAsync(e =>
                    e.ContextoEstadoId == contextoViaje.ContextoEstadoId &&
                    e.Nombre == "Cerrado");

            var contextoVehiculo = await _context.ContextosEstado
                .FirstAsync(c => c.Nombre == "VEHICULO");

            var estadoDisponible = await _context.Estados
                .FirstAsync(e =>
                    e.ContextoEstadoId == contextoVehiculo.ContextoEstadoId &&
                    e.Nombre == "Disponible");

            viaje.KilometrajeLlegada = request.KilometrajeLlegada;
            viaje.FechaHoraLlegada = DateTime.UtcNow;
            viaje.ObservacionLlegada = string.IsNullOrWhiteSpace(request.ObservacionLlegada)
                ? "Cierre forzado por administrador."
                : request.ObservacionLlegada.Trim();
            viaje.EstadoId = estadoCerrado.EstadoId;
            viaje.FechaActualizacion = DateTime.UtcNow;

            if (viaje.Vehiculo != null)
            {
                viaje.Vehiculo.KilometrajeActual = request.KilometrajeLlegada;
                viaje.Vehiculo.EstadoId = estadoDisponible.EstadoId;
                viaje.Vehiculo.FechaActualizacion = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static System.Linq.Expressions.Expression<Func<Viaje, ViajeDto>> MapViajeDto()
        {
            return v => new ViajeDto
            {
                ViajeId = v.ViajeId,
                UsuarioId = v.UsuarioId,
                NombreUsuario = v.Usuario != null ? v.Usuario.NombreUsuario : string.Empty,
                VehiculoId = v.VehiculoId,
                Patente = v.Vehiculo != null ? v.Vehiculo.Patente : string.Empty,
                VehiculoDescripcion = v.Vehiculo != null && v.Vehiculo.Modelo != null && v.Vehiculo.Modelo.Marca != null
                    ? v.Vehiculo.Modelo.Marca.Nombre + " " + v.Vehiculo.Modelo.Nombre
                    : string.Empty,
                EstadoId = v.EstadoId,
                EstadoNombre = v.Estado != null ? v.Estado.Nombre : string.Empty,
                FechaHoraSalida = v.FechaHoraSalida,
                FechaHoraLlegada = v.FechaHoraLlegada,
                KilometrajeSalida = v.KilometrajeSalida,
                KilometrajeLlegada = v.KilometrajeLlegada,
                ObservacionSalida = v.ObservacionSalida,
                ObservacionLlegada = v.ObservacionLlegada
            };
        }
    }
}
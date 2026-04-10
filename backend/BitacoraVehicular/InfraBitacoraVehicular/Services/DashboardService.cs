using AppBitacoraVehicular.DTOs.Dashboard;
using AppBitacoraVehicular.Interfaces;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfraBitacoraVehicular.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly BitacoraVehicularDbContext _context;

        public DashboardService(BitacoraVehicularDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardResumenDto> GetResumenAsync()
        {
            var totalVehiculos = await _context.Vehiculos.CountAsync(v => v.Activo);

            var vehiculosDisponibles = await _context.Vehiculos
                .Include(v => v.Estado)
                .CountAsync(v => v.Activo && v.Estado != null && v.Estado.Nombre == "Disponible");

            var vehiculosEnUso = await _context.Vehiculos
                .Include(v => v.Estado)
                .CountAsync(v => v.Activo && v.Estado != null && v.Estado.Nombre == "EnUso");

            var vehiculosNoDisponibles = await _context.Vehiculos
                .Include(v => v.Estado)
                .CountAsync(v => v.Activo && v.Estado != null &&
                    (v.Estado.Nombre == "Mantencion" ||
                     v.Estado.Nombre == "NoDisponible" ||
                     v.Estado.Nombre == "Deshabilitado"));

            var viajesAbiertos = await _context.Viajes
                .CountAsync(v => v.FechaHoraLlegada == null);

            var usuariosActivos = await _context.Usuarios
                .Include(u => u.Estado)
                .CountAsync(u => u.Activo && u.Estado != null && u.Estado.Nombre == "Activo");

            var usuariosPendientes = await _context.Usuarios
                .Include(u => u.Estado)
                .CountAsync(u => u.Activo && u.Estado != null && u.Estado.Nombre == "Pendiente");

            var asignacionesActivas = await _context.AsignacionesVehiculo
                .Include(a => a.Estado)
                .CountAsync(a => a.FechaTermino == null && a.Estado != null && a.Estado.Nombre == "Activa");

            return new DashboardResumenDto
            {
                TotalVehiculos = totalVehiculos,
                VehiculosDisponibles = vehiculosDisponibles,
                VehiculosEnUso = vehiculosEnUso,
                VehiculosNoDisponibles = vehiculosNoDisponibles,
                ViajesAbiertos = viajesAbiertos,
                UsuariosActivos = usuariosActivos,
                UsuariosPendientes = usuariosPendientes,
                AsignacionesActivas = asignacionesActivas
            };
        }

        public async Task<List<ViajeAbiertoDashboardDto>> GetViajesAbiertosAsync()
        {
            return await _context.Viajes
                .Include(v => v.Usuario)
                .Include(v => v.Vehiculo)
                    .ThenInclude(vh => vh!.Modelo)
                        .ThenInclude(m => m!.Marca)
                .Where(v => v.FechaHoraLlegada == null)
                .OrderByDescending(v => v.FechaHoraSalida)
                .Select(v => new ViajeAbiertoDashboardDto
                {
                    ViajeId = v.ViajeId,
                    UsuarioId = v.UsuarioId,
                    NombreUsuario = v.Usuario != null ? v.Usuario.NombreUsuario : string.Empty,
                    NombreCompleto = v.Usuario != null ? v.Usuario.NombreCompleto : string.Empty,
                    VehiculoId = v.VehiculoId,
                    Patente = v.Vehiculo != null ? v.Vehiculo.Patente : string.Empty,
                    VehiculoDescripcion = v.Vehiculo != null && v.Vehiculo.Modelo != null && v.Vehiculo.Modelo.Marca != null
                        ? v.Vehiculo.Modelo.Marca.Nombre + " " + v.Vehiculo.Modelo.Nombre
                        : string.Empty,
                    FechaHoraSalida = v.FechaHoraSalida,
                    KilometrajeSalida = v.KilometrajeSalida,
                    ObservacionSalida = v.ObservacionSalida
                })
                .ToListAsync();
        }

        public async Task<List<UsuarioPendienteDashboardDto>> GetUsuariosPendientesAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Estado)
                .Where(u => u.Estado != null && u.Estado.Nombre == "Pendiente")
                .OrderByDescending(u => u.FechaCreacion)
                .Select(u => new UsuarioPendienteDashboardDto
                {
                    UsuarioId = u.UsuarioId,
                    NombreCompleto = u.NombreCompleto,
                    Correo = u.Correo,
                    NombreUsuario = u.NombreUsuario,
                    FechaCreacion = u.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<List<VehiculoProximoMantencionDto>> GetVehiculosProximosMantencionAsync()
        {
            var hoy = DateTime.UtcNow.Date;

            return await _context.Mantenciones
                .Include(m => m.Vehiculo)
                    .ThenInclude(v => v!.Modelo)
                        .ThenInclude(mo => mo!.Marca)
                .Include(m => m.TipoMantencion)
                .Where(m =>
                    m.FechaSalida != null &&
                    (
                        (m.ProximaMantencionFecha.HasValue && m.ProximaMantencionFecha.Value.Date <= hoy.AddDays(30)) ||
                        (m.ProximaMantencionKm.HasValue && m.Vehiculo != null && m.Vehiculo.KilometrajeActual >= m.ProximaMantencionKm.Value - 500)
                    ))
                .OrderBy(m => m.ProximaMantencionFecha)
                .ThenBy(m => m.ProximaMantencionKm)
                .Select(m => new VehiculoProximoMantencionDto
                {
                    VehiculoId = m.VehiculoId,
                    Patente = m.Vehiculo != null ? m.Vehiculo.Patente : string.Empty,
                    VehiculoDescripcion = m.Vehiculo != null && m.Vehiculo.Modelo != null && m.Vehiculo.Modelo.Marca != null
                        ? m.Vehiculo.Modelo.Marca.Nombre + " " + m.Vehiculo.Modelo.Nombre
                        : string.Empty,
                    KilometrajeActual = m.Vehiculo != null ? m.Vehiculo.KilometrajeActual : 0,
                    ProximaMantencionKm = m.ProximaMantencionKm,
                    ProximaMantencionFecha = m.ProximaMantencionFecha,
                    TipoMantencionNombre = m.TipoMantencion != null ? m.TipoMantencion.Nombre : null
                })
                .ToListAsync();
        }

        public async Task<List<VehiculoFueraServicioDto>> GetVehiculosFueraServicioAsync()
        {
            return await _context.Vehiculos
                .Include(v => v.Estado)
                .Include(v => v.Modelo)
                    .ThenInclude(m => m!.Marca)
                .Where(v =>
                    v.Activo &&
                    v.Estado != null &&
                    (v.Estado.Nombre == "Mantencion" ||
                     v.Estado.Nombre == "NoDisponible" ||
                     v.Estado.Nombre == "Deshabilitado"))
                .OrderBy(v => v.Patente)
                .Select(v => new VehiculoFueraServicioDto
                {
                    VehiculoId = v.VehiculoId,
                    Patente = v.Patente,
                    VehiculoDescripcion = v.Modelo != null && v.Modelo.Marca != null
                        ? v.Modelo.Marca.Nombre + " " + v.Modelo.Nombre
                        : string.Empty,
                    EstadoNombre = v.Estado != null ? v.Estado.Nombre : string.Empty,
                    KilometrajeActual = v.KilometrajeActual,
                    Observacion = v.Observacion
                })
                .ToListAsync();
        }
    }
}

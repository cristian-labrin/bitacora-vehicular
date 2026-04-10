using AppBitacoraVehicular.DTOs.MisVehiculos;
using AppBitacoraVehicular.Interfaces;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfraBitacoraVehicular.Services
{
    public class MisVehiculosService : IMisVehiculosService
    {
        private readonly BitacoraVehicularDbContext _context;

        public MisVehiculosService(BitacoraVehicularDbContext context)
        {
            _context = context;
        }

        public async Task<List<MisVehiculosDto>> GetMisVehiculosAsync(int usuarioId)
        {
            return await _context.AsignacionesVehiculo
                .Include(a => a.Vehiculo)
                    .ThenInclude(v => v!.Modelo)
                        .ThenInclude(m => m!.Marca)
                .Include(a => a.Vehiculo)
                    .ThenInclude(v => v!.Estado)
                .Include(a => a.Estado)
                .Where(a =>
                    a.UsuarioId == usuarioId &&
                    a.FechaTermino == null &&
                    a.Estado != null &&
                    a.Estado.Nombre == "Activa" &&
                    a.Vehiculo != null &&
                    a.Vehiculo.Activo)
                .OrderByDescending(a => a.FechaAsignacion)
                .Select(a => new MisVehiculosDto
                {
                    VehiculoId = a.VehiculoId,
                    Patente = a.Vehiculo != null ? a.Vehiculo.Patente : string.Empty,
                    MarcaNombre = a.Vehiculo != null && a.Vehiculo.Modelo != null && a.Vehiculo.Modelo.Marca != null
                        ? a.Vehiculo.Modelo.Marca.Nombre
                        : string.Empty,
                    ModeloNombre = a.Vehiculo != null && a.Vehiculo.Modelo != null
                        ? a.Vehiculo.Modelo.Nombre
                        : string.Empty,
                    Anio = a.Vehiculo != null ? a.Vehiculo.Anio : 0,
                    Color = a.Vehiculo != null ? a.Vehiculo.Color : string.Empty,
                    KilometrajeActual = a.Vehiculo != null ? a.Vehiculo.KilometrajeActual : 0,
                    EstadoId = a.Vehiculo != null ? a.Vehiculo.EstadoId : 0,
                    EstadoNombre = a.Vehiculo != null && a.Vehiculo.Estado != null
                        ? a.Vehiculo.Estado.Nombre
                        : string.Empty,
                    FechaAsignacion = a.FechaAsignacion,
                    ObservacionAsignacion = a.Observacion,
                    TieneViajeAbierto = _context.Viajes.Any(v =>
                        v.UsuarioId == usuarioId &&
                        v.VehiculoId == a.VehiculoId &&
                        v.FechaHoraLlegada == null),
                    ViajeAbiertoId = _context.Viajes
                        .Where(v =>
                            v.UsuarioId == usuarioId &&
                            v.VehiculoId == a.VehiculoId &&
                            v.FechaHoraLlegada == null)
                        .Select(v => (int?)v.ViajeId)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }
    }
}

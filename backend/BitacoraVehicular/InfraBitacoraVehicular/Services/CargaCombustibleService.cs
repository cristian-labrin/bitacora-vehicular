using AppBitacoraVehicular.DTOs.CargaCombustible;
using AppBitacoraVehicular.Interfaces;
using DomainBitacoraVehicular.Entities;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfraBitacoraVehicular.Services
{
    public class CargaCombustibleService : ICargaCombustibleService
    {
        private readonly BitacoraVehicularDbContext _context;

        public CargaCombustibleService(BitacoraVehicularDbContext context)
        {
            _context = context;
        }

        public async Task<List<CargaCombustibleDto>> GetMisCargasAsync(int usuarioId)
        {
            return await _context.CargasCombustible
                .Include(c => c.Usuario)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v!.Modelo)
                        .ThenInclude(m => m!.Marca)
                .Include(c => c.TipoCombustible)
                .Where(c => c.UsuarioId == usuarioId)
                .OrderByDescending(c => c.FechaCarga)
                .Select(MapDto())
                .ToListAsync();
        }

        public async Task<List<CargaCombustibleDto>> GetAllAsync()
        {
            return await _context.CargasCombustible
                .Include(c => c.Usuario)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v!.Modelo)
                        .ThenInclude(m => m!.Marca)
                .Include(c => c.TipoCombustible)
                .OrderByDescending(c => c.FechaCarga)
                .Select(MapDto())
                .ToListAsync();
        }

        public async Task<CargaCombustibleDto?> GetByIdAsync(int id, int usuarioId, bool esAdmin)
        {
            var query = _context.CargasCombustible
                .Include(c => c.Usuario)
                .Include(c => c.Vehiculo)
                    .ThenInclude(v => v!.Modelo)
                        .ThenInclude(m => m!.Marca)
                .Include(c => c.TipoCombustible)
                .Where(c => c.CargaCombustibleId == id);

            if (!esAdmin)
                query = query.Where(c => c.UsuarioId == usuarioId);

            return await query
                .Select(MapDto())
                .FirstOrDefaultAsync();
        }

        public async Task<CargaCombustibleDto> CreateAsync(int usuarioId, CreateCargaCombustibleDto request)
        {
            if (request.MontoCargado <= 0)
                throw new InvalidOperationException("El monto cargado es obligatorio y debe ser mayor a cero.");

            var tipoCombustibleExiste = await _context.TiposCombustible
                .AnyAsync(t => t.TipoCombustibleId == request.TipoCombustibleId && t.Activo);

            if (!tipoCombustibleExiste)
                throw new InvalidOperationException("El tipo de combustible no existe o está inactivo.");

            var viaje = await _context.Viajes
                .Include(v => v.Vehiculo)
                .FirstOrDefaultAsync(v => v.ViajeId == request.ViajeId);

            if (viaje is null)
                throw new InvalidOperationException("El viaje no existe.");

            if (viaje.UsuarioId != usuarioId)
                throw new InvalidOperationException("No puedes registrar carga para un viaje de otro usuario.");

            var yaExisteCarga = await _context.CargasCombustible
                .AnyAsync(c => c.ViajeId == request.ViajeId);

            if (yaExisteCarga)
                throw new InvalidOperationException("El viaje ya tiene una carga de combustible registrada.");

            var carga = new CargaCombustible
            {
                ViajeId = request.ViajeId,
                UsuarioId = usuarioId,
                VehiculoId = viaje.VehiculoId,
                TipoCombustibleId = request.TipoCombustibleId,
                Litros = request.Litros,
                MontoCargado = request.MontoCargado,
                FechaCarga = DateTime.UtcNow,
                NombreArchivoBoleta = request.NombreArchivoBoleta?.Trim(),
                RutaArchivoBoleta = request.RutaArchivoBoleta?.Trim(),
                Observacion = request.Observacion?.Trim(),
                FechaCreacion = DateTime.UtcNow
            };

            _context.CargasCombustible.Add(carga);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(carga.CargaCombustibleId, usuarioId, true)
                ?? throw new InvalidOperationException("No se pudo recuperar la carga creada.");
        }

        private static System.Linq.Expressions.Expression<Func<CargaCombustible, CargaCombustibleDto>> MapDto()
        {
            return c => new CargaCombustibleDto
            {
                CargaCombustibleId = c.CargaCombustibleId,
                ViajeId = c.ViajeId,
                UsuarioId = c.UsuarioId,
                NombreUsuario = c.Usuario != null ? c.Usuario.NombreUsuario : string.Empty,
                VehiculoId = c.VehiculoId,
                Patente = c.Vehiculo != null ? c.Vehiculo.Patente : string.Empty,
                VehiculoDescripcion = c.Vehiculo != null && c.Vehiculo.Modelo != null && c.Vehiculo.Modelo.Marca != null
                    ? c.Vehiculo.Modelo.Marca.Nombre + " " + c.Vehiculo.Modelo.Nombre
                    : string.Empty,
                TipoCombustibleId = c.TipoCombustibleId,
                TipoCombustibleNombre = c.TipoCombustible != null ? c.TipoCombustible.Nombre : string.Empty,
                Litros = c.Litros,
                MontoCargado = c.MontoCargado,
                FechaCarga = c.FechaCarga,
                NombreArchivoBoleta = c.NombreArchivoBoleta,
                RutaArchivoBoleta = c.RutaArchivoBoleta,
                Observacion = c.Observacion
            };
        }
    }
}
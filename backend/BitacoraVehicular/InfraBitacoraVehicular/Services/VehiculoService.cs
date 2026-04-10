using AppBitacoraVehicular.DTOs.Marca;
using AppBitacoraVehicular.DTOs.Vehiculo;
using AppBitacoraVehicular.Interfaces;
using DomainBitacoraVehicular.Entities;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfraBitacoraVehicular.Services
{
    public class VehiculoService : IVehiculoService
    {
        private readonly BitacoraVehicularDbContext _context;

        public VehiculoService(BitacoraVehicularDbContext context)
        {
            _context = context;
        }

        public async Task<List<VehiculoDto>> GetAllAsync()
        {
            return await _context.Vehiculos
                .Include(v => v.Modelo)
                    .ThenInclude(m => m!.Marca)
                .Include(v => v.Estado)
                .OrderBy(v => v.Patente)
                .Select(v => new VehiculoDto
                {
                    VehiculoId = v.VehiculoId,
                    ModeloId = v.ModeloId,
                    MarcaNombre = v.Modelo != null && v.Modelo.Marca != null ? v.Modelo.Marca.Nombre : string.Empty,
                    ModeloNombre = v.Modelo != null ? v.Modelo.Nombre : string.Empty,
                    EstadoId = v.EstadoId,
                    EstadoNombre = v.Estado != null ? v.Estado.Nombre : string.Empty,
                    Patente = v.Patente,
                    Anio = v.Anio,
                    Color = v.Color,
                    KilometrajeActual = v.KilometrajeActual,
                    Observacion = v.Observacion,
                    Activo = v.Activo
                })
                .ToListAsync();
        }

        public async Task<VehiculoDto?> GetByIdAsync(int id)
        {
            return await _context.Vehiculos
                .Include(v => v.Modelo)
                    .ThenInclude(m => m!.Marca)
                .Include(v => v.Estado)
                .Where(v => v.VehiculoId == id)
                .Select(v => new VehiculoDto
                {
                    VehiculoId = v.VehiculoId,
                    ModeloId = v.ModeloId,
                    MarcaNombre = v.Modelo != null && v.Modelo.Marca != null ? v.Modelo.Marca.Nombre : string.Empty,
                    ModeloNombre = v.Modelo != null ? v.Modelo.Nombre : string.Empty,
                    EstadoId = v.EstadoId,
                    EstadoNombre = v.Estado != null ? v.Estado.Nombre : string.Empty,
                    Patente = v.Patente,
                    Anio = v.Anio,
                    Color = v.Color,
                    KilometrajeActual = v.KilometrajeActual,
                    Observacion = v.Observacion,
                    Activo = v.Activo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<VehiculoDto> CreateAsync(CreateVehiculoDto request)
        {
            var modeloExiste = await _context.Modelos.AnyAsync(m => m.ModeloId == request.ModeloId && m.Activo);
            if (!modeloExiste)
                throw new InvalidOperationException("El modelo no existe o está inactivo.");

            var estadoExiste = await _context.Estados.AnyAsync(e => e.EstadoId == request.EstadoId);
            if (!estadoExiste)
                throw new InvalidOperationException("El estado no existe.");

            var patente = request.Patente.Trim().ToUpper();

            var existePatente = await _context.Vehiculos.AnyAsync(v => v.Patente == patente);
            if (existePatente)
                throw new InvalidOperationException("La patente ya existe.");

            var vehiculo = new Vehiculo
            {
                ModeloId = request.ModeloId,
                EstadoId = request.EstadoId,
                Patente = patente,
                Anio = request.Anio,
                Color = request.Color.Trim(),
                KilometrajeActual = request.KilometrajeActual,
                Observacion = request.Observacion?.Trim(),
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(vehiculo.VehiculoId)
                ?? throw new InvalidOperationException("No se pudo recuperar el vehículo creado.");
        }

        public async Task<bool> UpdateAsync(int id, UpdateVehiculoDto request)
        {
            var vehiculo = await _context.Vehiculos.FirstOrDefaultAsync(v => v.VehiculoId == id);
            if (vehiculo is null)
                return false;

            var modeloExiste = await _context.Modelos.AnyAsync(m => m.ModeloId == request.ModeloId && m.Activo);
            if (!modeloExiste)
                throw new InvalidOperationException("El modelo no existe o está inactivo.");

            var estadoExiste = await _context.Estados.AnyAsync(e => e.EstadoId == request.EstadoId);
            if (!estadoExiste)
                throw new InvalidOperationException("El estado no existe.");

            var patente = request.Patente.Trim().ToUpper();

            var existePatente = await _context.Vehiculos
                .AnyAsync(v => v.VehiculoId != id && v.Patente == patente);

            if (existePatente)
                throw new InvalidOperationException("Ya existe otro vehículo con esa patente.");

            vehiculo.ModeloId = request.ModeloId;
            vehiculo.EstadoId = request.EstadoId;
            vehiculo.Patente = patente;
            vehiculo.Anio = request.Anio;
            vehiculo.Color = request.Color.Trim();
            vehiculo.KilometrajeActual = request.KilometrajeActual;
            vehiculo.Observacion = request.Observacion?.Trim();
            vehiculo.Activo = request.Activo;
            vehiculo.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehiculo = await _context.Vehiculos.FirstOrDefaultAsync(v => v.VehiculoId == id);
            if (vehiculo is null)
                return false;

            vehiculo.Activo = false;
            vehiculo.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

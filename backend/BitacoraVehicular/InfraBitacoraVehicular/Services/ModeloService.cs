using AppBitacoraVehicular.DTOs.Marca;
using AppBitacoraVehicular.DTOs.Modelo;
using AppBitacoraVehicular.Interfaces;
using DomainBitacoraVehicular.Entities;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfraBitacoraVehicular.Services
{
    public class ModeloService : IModeloService
    {
        private readonly BitacoraVehicularDbContext _context;

        public ModeloService(BitacoraVehicularDbContext context)
        {
            _context = context;
        }

        public async Task<List<ModeloDto>> GetAllAsync()
        {
            return await _context.Modelos
                .Include(m => m.Marca)
                .OrderBy(m => m.Marca!.Nombre)
                .ThenBy(m => m.Nombre)
                .Select(m => new ModeloDto
                {
                    ModeloId = m.ModeloId,
                    MarcaId = m.MarcaId,
                    MarcaNombre = m.Marca != null ? m.Marca.Nombre : string.Empty,
                    Nombre = m.Nombre,
                    Activo = m.Activo
                })
                .ToListAsync();
        }

        public async Task<ModeloDto?> GetByIdAsync(int id)
        {
            return await _context.Modelos
                .Include(m => m.Marca)
                .Where(m => m.ModeloId == id)
                .Select(m => new ModeloDto
                {
                    ModeloId = m.ModeloId,
                    MarcaId = m.MarcaId,
                    MarcaNombre = m.Marca != null ? m.Marca.Nombre : string.Empty,
                    Nombre = m.Nombre,
                    Activo = m.Activo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ModeloDto> CreateAsync(CreateModeloDto request)
        {
            var marcaExiste = await _context.Marcas.AnyAsync(m => m.MarcaId == request.MarcaId && m.Activo);
            if (!marcaExiste)
                throw new InvalidOperationException("La marca no existe o está inactiva.");

            var nombre = request.Nombre.Trim();

            var existe = await _context.Modelos
                .AnyAsync(m => m.MarcaId == request.MarcaId && m.Nombre == nombre);

            if (existe)
                throw new InvalidOperationException("El modelo ya existe para esa marca.");

            var modelo = new Modelo
            {
                MarcaId = request.MarcaId,
                Nombre = nombre,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Modelos.Add(modelo);
            await _context.SaveChangesAsync();

            var marca = await _context.Marcas.FirstAsync(m => m.MarcaId == modelo.MarcaId);

            return new ModeloDto
            {
                ModeloId = modelo.ModeloId,
                MarcaId = modelo.MarcaId,
                MarcaNombre = marca.Nombre,
                Nombre = modelo.Nombre,
                Activo = modelo.Activo
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateModeloDto request)
        {
            var modelo = await _context.Modelos.FirstOrDefaultAsync(m => m.ModeloId == id);
            if (modelo is null)
                return false;

            var marcaExiste = await _context.Marcas.AnyAsync(m => m.MarcaId == request.MarcaId && m.Activo);
            if (!marcaExiste)
                throw new InvalidOperationException("La marca no existe o está inactiva.");

            var nombre = request.Nombre.Trim();

            var existe = await _context.Modelos
                .AnyAsync(m => m.ModeloId != id && m.MarcaId == request.MarcaId && m.Nombre == nombre);

            if (existe)
                throw new InvalidOperationException("Ya existe otro modelo con ese nombre para esa marca.");

            modelo.MarcaId = request.MarcaId;
            modelo.Nombre = nombre;
            modelo.Activo = request.Activo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var modelo = await _context.Modelos.FirstOrDefaultAsync(m => m.ModeloId == id);
            if (modelo is null)
                return false;

            modelo.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
